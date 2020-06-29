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
        OnChannelSelectEnterChannel = 5,
        OnCheckUserLimitPacket = 6,

        // CLogin::OnAcceptLicense
        // CLogin::OnDenyLicense
        OnLicenseResult = 7,
        OnSetGenderPacket = 8,
        OnPinCodeResult = 9,
        OnWorldListRequest = 11,
        OnViewAllCharPacket = 13,
        OnViewAllCharDlgResult = 15,
        OnCheckDuplicateIdPacket = 21,
        OnSendBackupPacket = 36,
    }
}