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

    public enum CLogin : short {
        OnCheckPasswordResult = 1,
        OnWorldListReinitializeRequest = 4,
        OnChannelSelectEnterChannel = 5,
        OnCheckUserLimitPacket = 6,

        // CLogin::OnAcceptLicense
        // CLogin::OnDenyLicense
        OnLicenseResult = 7,
        OnSetGenderPacket = 8,
        OnPinCodeResult = 9,
        OnWorldListRequest = 11,
        OnResetViewAllCharPacket = 12,
        OnViewAllCharPacket = 13,
        OnViewAllCharDlgResult = 15,
        OnSelectCharPacket = 19,
        OnEnterGamePacket = 20, // not the actual name
        OnCheckDuplicateIdPacket = 21,
        OnNewCharPacket22 = 22,
        OnNewCharPacket23 = 23,
        OnSelectCharInitSPWPacket = 28,
        OnSelectCharSPWPacket = 29,
        OnSendBackupPacket = 36,
        // CWvsContext::UI_Menu
        OnUiMenu = 218,
    }
}