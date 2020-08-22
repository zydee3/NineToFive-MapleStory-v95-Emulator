#include "stdafx.h"
#include "Discord.h"
#include "discord_rpc.h"
#include <time.h>

LPCSTR Discord::m_sApplicationID = "717245804628410368";
time_t Discord::m_nStartTimestamp = time(0);

void Discord::StartThread() {
	CreateThread(0, 0, (LPTHREAD_START_ROUTINE)Discord::Login, 0, 0, 0);
}

void Discord::Login() {
	DiscordEventHandlers handlers;
	memset(&handlers, 0, sizeof(handlers));
	Discord_Initialize(Discord::m_sApplicationID, &handlers, 1, NULL);
	while (1) {
		Discord::Update();
		Sleep(1000 * 60);
	}
}

void Discord::Update() {
	DiscordRichPresence rp;
	memset(&rp, 0, sizeof(rp));

	rp.startTimestamp = Discord::m_nStartTimestamp;
	rp.largeImageKey = "maplestory";
	rp.largeImageText = "by izarooni";
	rp.details = "haha your mom";
	rp.state = "made you look!";
	Discord_UpdatePresence(&rp);
}