// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "Hooker.h"
#include "Discord.h"

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
		//CreateConsole();
		Hooker::Hook_GetModuleFileNameW();
		std::cout << "::GetModuleFileNameW hooked" << std::endl;
		Hooker::NMCO_InitializeProxy();
		std::cout << "NMCO library loaded" << std::endl;
		Discord::StartThread();

		// remove chat restrictions
		Memory::WriteByte(0x004AA5B4, 0xEB);
		char update[] = {0xe9, 0xbf, 0, 0, 0, 0x90};
		memcpy((void*) 0x004AA707, &update, sizeof(update));
		Memory::WriteByte(0x004AA7EF, 0xEB);
		Memory::FillBytes(0x004290C3, 0x90, 2);
		break;
	}
	default: break;
	case DLL_PROCESS_DETACH:
		ExitProcess(0);
	}
	return TRUE;
}