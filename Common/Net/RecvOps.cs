using System;
using System.Collections.Generic;

namespace NineToFive.ReceiveOps {
    public class RecvOps {
        public Dictionary<short, Type> Events { get; private set; }
        public RecvOps() {
            Events = new Dictionary<short, Type>(35);
        }
    }

    public enum CLogin : short {
        OnCheckPasswordResult = 1,
        // CLogin::OnAcceptLicense
        // CLogin::OnDenyLicense
        OnLicenseResult = 7,
        OnSetGenderPacket = 8,
        OnPinCodeResult = 9,
        OnWorldListRequest = 11,
        OnSendBackupPacket = 36,
    }
}
