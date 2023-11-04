#include "Proxy.h"
#include "Config.h"
#include <Windows.h>
#include <thread>
#include <detours.h>
#include "HelperMarcros.h"
#include "Utilities.h"

struct MonoDomain;
struct MonoImage;
struct MonoClass;
struct MonoAssembly;
struct MonoMethod;
struct MonoObject;

MAKE_FUNCTION_DECL(mono_get_domain, MonoDomain*,);
MAKE_FUNCTION_DECL(mono_jit_init_version, void*, const char*, const char*);
MAKE_FUNCTION_DECL(mono_get_root_domain, MonoDomain*,);
MAKE_FUNCTION_DECL(mono_domain_assembly_open, MonoAssembly*, MonoDomain*, const char*);
MAKE_FUNCTION_DECL(mono_runtime_invoke, MonoObject*, MonoMethod*, void*, void**, MonoObject**);
MAKE_FUNCTION_DECL(mono_method_get_name, const char*, const void*);

MAKE_FUNCTION_DECL(mono_assembly_get_image, MonoImage*, MonoAssembly*);
MAKE_FUNCTION_DECL(mono_class_from_name, MonoClass*, MonoImage*, const char*, const char*);
MAKE_FUNCTION_DECL(mono_class_get_method_from_name, MonoMethod*, MonoClass*, const char*, int);

DETOUR_FUNC(mono_runtime_invoke, MonoObject*, MonoMethod* method, void* obj, void** params, MonoObject** exc)
{
	const char* methodName = mono_method_get_name(method);

	if (string_ends_with(methodName, "Start"))
	{
		MonoDomain* domain = mono_get_root_domain();
		MonoAssembly* assembly = mono_domain_assembly_open(domain, "AddonManager/S.AddonsOverhaul.dll");
		MonoImage* image = mono_assembly_get_image(assembly);
		MonoClass* getClass = mono_class_from_name(image, "S.AddonsOverhaul", "Loader");
		MonoMethod* loaderMethod = mono_class_get_method_from_name(getClass, "Load", 0);
		p_mono_runtime_invoke(loaderMethod, obj, params, exc);

		MonoObject* result = p_mono_runtime_invoke(method, obj, params, exc);
		UNHOOK_FUNC(mono_runtime_invoke);
		return result;
	}
	return p_mono_runtime_invoke(method, obj, params, exc);
}

DETOUR_STDFUNC(LoadLibraryW, HMODULE, LPCWSTR lpLibFileName)
{
	HMODULE library = p_LoadLibraryW(lpLibFileName);

	if (wcsstr(lpLibFileName, MONO_ASSEMBLY))
	{
		BIND_FUNCTION(library, mono_runtime_invoke);
		BIND_FUNCTION(library, mono_method_get_name);
		BIND_FUNCTION(library, mono_domain_assembly_open);
		BIND_FUNCTION(library, mono_get_root_domain);
		BIND_FUNCTION(library, mono_jit_init_version);
		BIND_FUNCTION(library, mono_get_domain);
		BIND_FUNCTION(library, mono_assembly_get_image);
		BIND_FUNCTION(library, mono_class_from_name);
		BIND_FUNCTION(library, mono_class_get_method_from_name);
		HOOK_FUNC(mono_runtime_invoke);
		UNHOOK_FUNC(LoadLibraryW);
	}

	return library;
}

BOOL WINAPI DllMain(
	HINSTANCE,
	DWORD fdwReason,
	LPVOID)
{
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
		Proxy::Initialize();

		if (!(Proxy::IsGameProcess() || Proxy::IsServerProcess()))
			return TRUE;

		DetourRestoreAfterWith();

		HOOK_FUNC(LoadLibraryW);
	}

	return TRUE;
}
