#include "stdafx.h"
#include "Hooker.h"

void Hooker::Hook_GetModuleFileNameW() {
	static decltype(&GetModuleFileNameW) _GetModuleFileNameW = &GetModuleFileNameW;

	const decltype(&GetModuleFileNameW) GetModuleFileNameW_Hook = [](HMODULE hModule, LPWSTR lpFileName, DWORD dwSize) -> DWORD {
		auto len = _GetModuleFileNameW(hModule, lpFileName, dwSize);
		// Check to see if the length is invalid (zero)
		if (!len) {
			// Try again without the provided module for a fixed result
			len = _GetModuleFileNameW(nullptr, lpFileName, dwSize);
		}
		return len;
	};

	Memory::SetHook(true, reinterpret_cast<void**>(&_GetModuleFileNameW), GetModuleFileNameW_Hook);
}

FARPROC dwNMCOCallFunc;
FARPROC dwNMCOMemoryFree;

void Hooker::NMCO_InitializeProxy() {
	HMODULE hModule = LoadLibraryA("nmconew2.dll");
	if (hModule == nullptr) {
		MessageBox(NULL, L"Failed to find nmconew2.dll file", L"Missing file", 0);
		return;
	}
	dwNMCOCallFunc = GetProcAddress(hModule, "NMCO_CallNMFunc");
	dwNMCOMemoryFree = GetProcAddress(hModule, "NMCO_MemoryFree");
}

extern "C" __declspec(dllexport) __declspec(naked) void NMCO_CallNMFunc() {
	__asm jmp dwNMCOCallFunc
}

extern "C" __declspec(dllexport) __declspec(naked) void NMCO_MemoryFree() {
	__asm jmp dwNMCOMemoryFree
}