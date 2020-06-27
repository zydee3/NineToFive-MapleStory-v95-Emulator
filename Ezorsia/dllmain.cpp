// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "Hooker.h"

void CreateConsole() {
	AllocConsole();
	FILE* stream;
	freopen_s(&stream, "CONOUT$", "w", stdout);
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
	{
		CreateConsole();
		Hooker::Hook_GetModuleFileNameW();
		std::cout << "::GetModuleFileNameW hooked" << std::endl;
		Hooker::NMCO_InitializeProxy();
		std::cout << "NMCO library loaded" << std::endl;
		break;
	}
	default: break;
	case DLL_PROCESS_DETACH:
		ExitProcess(0);
	}
	return TRUE;
}