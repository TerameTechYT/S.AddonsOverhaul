#pragma once

#ifndef _WIN64
# error "This DLL wrapper only works on Windows!"
#endif

class Proxy
{
public:
	static bool IsGameProcess();
	static bool IsServerProcess();
	static void Initialize();
};
