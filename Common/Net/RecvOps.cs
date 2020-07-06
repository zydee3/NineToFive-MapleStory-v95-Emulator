using System;
using System.Collections.Generic;

namespace NineToFive.ReceiveOps {
    public class RecvOps {
        public Dictionary<short, Type> Events { get; }

        public RecvOps() {
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

        CWvsContext_OnCheckOpBoardHasNew = 192,
        CWvsContext_OnUiMenu = 218,

        User_OnUserMove = 44,
        User_OnChatMsg = 54,

        UserLocal_OnPortalCollision = 112,
        UserLocal_OnResetNLCPQ = 251,
    }
}