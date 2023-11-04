using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace S.AddonsOverhaul.Patcher.Core.Patchers
{
    internal class MonoPatcher : IGamePatcher
    {
        public const string AssemblyDir = "Managed";
        public const string AssemblyName = "Assembly-CSharp.dll";

        private const string
            TargetType =
                "Assets.Scripts.GameManager";

        private const string TargetFunction = "OnEnable";

        private const string Signature = "StationeersModLoader";

        private AssemblyDefinition _assembly;
        private ModuleDefinition _module;
        private TypeDefinition _type;

        public string AssemblyFileName;
        public string TemporaryAssemblyFileName;

        public void Load(string instanceExe)
        {
            GetInstanceAssemblies(instanceExe);

            if (!File.Exists(AssemblyFileName))
                Logger.Current.LogFatal($"Could not find game/server assembly '{AssemblyFileName}'.");

            File.Copy(AssemblyFileName, TemporaryAssemblyFileName, true);

            Logger.Current.Log($"Copied game/server assembly into temporary file '{TemporaryAssemblyFileName}'");

            _assembly = AssemblyDefinition.ReadAssembly(TemporaryAssemblyFileName);

            if (_assembly == null)
            {
                Logger.Current.LogFatal($"Could not read game/server assembly '{TemporaryAssemblyFileName}'.");
                return;
            }

            _module = _assembly.MainModule;

            if (_module == null)
            {
                Logger.Current.LogFatal(
                    $"Could not read game assembly (MainModule not found) '{TemporaryAssemblyFileName}'.");
                return;
            }

            Logger.Current.Log($"Found module: {_module.FileName}");

            _type = _module.Types.FirstOrDefault(x => x.FullName == TargetType);

            if (_type == null)
                Logger.Current.LogFatal($"Could not find target type '{TargetType}'. " +
                                        "Please make sure that you have the latest version of Stationeers.ModLoader!");
        }

        public void Dispose()
        {
            _assembly?.Dispose();

            if (File.Exists(TemporaryAssemblyFileName))
            {
                Logger.Current.Log($"Deleting temporary assembly file '{TemporaryAssemblyFileName}'");
                File.Delete(TemporaryAssemblyFileName);
            }
        }

        public void Patch()
        {
            if (IsPatched())
            {
                Logger.Current.Log("Game is already patched.");
                Console.ReadLine();
                return;
            }

            Backup();

            Inject();
        }

        public bool IsPatched()
        {
            var signature = _type.Fields.FirstOrDefault(x => x.Name == Signature);

            if (signature != null)
                return true;

            return false;
        }

        private void GetInstanceAssemblies(string installInstance)
        {
            string installResourcesDir = null;
            if (installInstance == Constants.GameExe)
                installResourcesDir = Constants.GameResourcesDir;
            else if (installInstance == Constants.ServerExe) installResourcesDir = Constants.ServerResourcesDir;
            Debug.Assert(!string.IsNullOrEmpty(installResourcesDir), "Invalid install dir!");

            AssemblyFileName =
                Path.Combine(Environment.CurrentDirectory, installResourcesDir, AssemblyDir, AssemblyName);
            TemporaryAssemblyFileName = Path.Combine(Environment.CurrentDirectory, installResourcesDir, AssemblyDir,
                AssemblyName + ".temp.dll");
        }

        private void Backup()
        {
            File.Copy(AssemblyFileName, AssemblyFileName + ".backup", true);

            if (!File.Exists(AssemblyFileName + ".original"))
                File.Copy(AssemblyFileName, AssemblyFileName + ".original", false);
        }

        private void Inject()
        {
            Logger.Current.Log("Injecting...");

            var voidType = _module.ImportReference(typeof(void));

            Logger.Current.Log("Creating method definition");
            var method = new MethodDefinition(TargetFunction, MethodAttributes.Private, voidType);

            method.Body = new MethodBody(method);
            method.Body.Instructions.Clear();

            var methodBody = method.Body;

            var processor = methodBody.GetILProcessor();

            CreateLoaderMethodBody(ref processor, ref method);

            _type.Methods.Add(method);

            Logger.Current.Log("Creating loader signature");
            _type.Fields.Add(new FieldDefinition(Signature, FieldAttributes.Private,
                _module.ImportReference(typeof(int))));

            var cd = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetDirectoryName(AssemblyFileName);

            Logger.Current.Log("Saving modified assembly");
            _assembly.Write(AssemblyName);

            Environment.CurrentDirectory = cd;

            Logger.Current.Log("Successfully patched!");
        }

        private void CreateLoaderMethodBody(ref ILProcessor processor, ref MethodDefinition method)
        {
            Logger.Current.Log("Creating method body");

            var loadFile = _module.ImportReference(typeof(Assembly).GetMethod("LoadFile", new[]
            {
                typeof(string)
            }));

            var getType = _module.ImportReference(typeof(Assembly).GetMethod("GetType", new[]
            {
                typeof(string)
            }));

            var createInstance = _module.ImportReference(typeof(Activator).GetMethod("CreateInstance", new[]
            {
                typeof(Type)
            }));

            var invokeMember = _module.ImportReference(typeof(Type).GetMethod("InvokeMember", new[]
            {
                typeof(string),
                typeof(BindingFlags),
                typeof(Binder),
                typeof(object),
                typeof(object[])
            }));

            method.Body.Variables.Add(new VariableDefinition(_module.ImportReference(typeof(Type))));
            method.Body.Variables.Add(new VariableDefinition(_module.ImportReference(typeof(object))));

            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderAssemblyFileName));
            processor.Append(processor.Create(OpCodes.Call, loadFile));

            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderTypeName));
            processor.Append(processor.Create(OpCodes.Callvirt, getType));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc_0));

            processor.Append(processor.Create(OpCodes.Call, createInstance));
            processor.Append(processor.Create(OpCodes.Stloc_1));
            processor.Append(processor.Create(OpCodes.Ldloc_0));

            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderFunctionName));
            processor.Append(processor.Create(OpCodes.Ldc_I4, (int)BindingFlags.InvokeMethod));

            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(OpCodes.Ldloc_1));
            processor.Append(processor.Create(OpCodes.Ldnull));

            processor.Append(processor.Create(OpCodes.Callvirt, invokeMember));

            processor.Append(processor.Create(OpCodes.Pop));
            processor.Append(processor.Create(OpCodes.Ret));
        }
    }
}