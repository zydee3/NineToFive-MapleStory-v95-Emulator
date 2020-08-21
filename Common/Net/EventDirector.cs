using System;
using System.Collections.Generic;

namespace NineToFive.Net {
    public class EventDirector {
        public Dictionary<short, Type> Events { get; }

        public EventDirector() {
            Events = new Dictionary<short, Type>(35);
        }

        public Type this[short i] {
            set => Events[i] = value;
        }
    }

    // ReSharper disable InconsistentNaming
    public enum ReceiveOperations : short {
        #region CLogin

        Login_OnCheckPasswordResult = 1,
        Login_OnWorldListReinitializeRequest = 4,
        Login_OnChannelSelectEnterChannel = 5,
        Login_OnCheckUserLimitPacket = 6,

        // CLogin::OnAcceptLicense
        // CLogin::OnDenyLicense
        Login_OnLicenseResult = 7,
        Login_OnSetGenderPacket = 8,
        Login_OnPinCodeResult = 9,
        Login_OnWorldListRequest = 11,
        Login_OnResetViewAllCharPacket = 12,
        Login_OnViewAllCharPacket = 13,
        Login_OnViewAllCharDlgResult = 15,
        Login_OnSelectCharPacket = 19,
        Login_OnEnterGamePacket = 20, // not the actual name
        Login_OnCheckDuplicateIdPacket = 21,
        Login_OnNewCharPacket22 = 22,
        Login_OnNewCharPacket23 = 23,
        Login_OnSelectCharInitSPWPacket = 28,
        Login_OnSelectCharSPWPacket = 29,
        Login_OnSendBackupPacket = 36,

        #endregion

        // CField, CCashShop, CITC
        OnTransferFieldRequest = 41,

        CWvsContext_SendAbilityUpRequest = 98,
        CWvsContext_SendSkillUpRequest = 102,
        CWvsContext_SendCharacterInfoRequest = 109,
        CWvsContext_OnCheckOpBoardHasNew = 192,
        CWvsContext_OnUiMenu = 218,

        User_OnUserMove = 44,
        User_OnChatMsg = 54,
        User_OnEmotion = 56,

        UserLocal_SetDamaged = 52,
        UserLocal_TalkToNpc = 63,
        UserLocal_ContinueTalkToNpc = 65,
        UserLocal_SendSkillUseRequest = 103,
        UserLocal_SendSkillCancelRequest = 104,
        UserLocal_OnPortalCollision = 112,
        UserLocal_TryRegisterTeleport = 113,
        UserLocal_UpdatePassiveSkillData = 217, 
        UserLocal_OnResetNLCPQ = 251,
        UserLocal_OnMeleeAttack = 47,
        
        Field_SendChatMsgSlash = 151,
        Field_LogChatMsgSlash = 152,
        
        Mob_GenerateMovePath = 227,
        
        CQuickslotKeyMappedMan_SaveQuickslotKeyMap = 216,
        CFuncKeyMappedMan_SaveFuncKeyMap = 159,
        
        
        CScriptMan_OnScriptMessage = 363,
    }
}