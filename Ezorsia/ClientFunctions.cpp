#include "stdafx.h"
#include "ClientFunctions.h"

typedef bool(_cdecl* pfnis_skill_need_master_level)(signed int);
auto is_skill_need_master_level = (pfnis_skill_need_master_level)(0x0047CCB0);

bool ClientFunctions::IsSkillNeedMasterLevel(signed int skillId) {
	return is_skill_need_master_level(skillId);
}
