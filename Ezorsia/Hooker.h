#pragma once
class Hooker {
public:
	static void Hook_GetModuleFileNameW();
	static void NMCO_InitializeProxy();
};

