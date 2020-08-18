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
		
		// CUserLocal__TryDoingMeleeAttack CWvsContext::IsUserGM
		Memory::WriteByte(0x0091E848, 0xEB);
		// CUserLocal__TryDoingNormalAttack CWvsContext__IsUserGM
		Memory::WriteByte(0x00912452, 0xEB);
		// CUserLocal__TryDoingShootAttack CWvsContext__IsUserGM
		Memory::WriteByte(0x00925B53, 0xEB);
		// CUserLocal__TryDoingMagicAttack CWvsContext__IsUserGM
		Memory::WriteByte(0x0092A2C3, 0xEB);
		// CUserLocal__TryDoingBodyAttack CWvsContext__IsUserGM
		Memory::WriteByte(0x0093075F, 0xEB);
		
		// CUISkill::OnSkillLevelUpButton
		Memory::WriteByte(0x0084D817, 90);
		Memory::WriteByte(0x0084D817 + 1, 0xEB);
		break;
	}
	default: break;
	case DLL_PROCESS_DETACH:
		ExitProcess(0);
	}
	return TRUE;
}