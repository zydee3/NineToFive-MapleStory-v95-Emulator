using System;
using NineToFive.Event;
using NineToFive.Login.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;

namespace NineToFive.Login {
    class LoginServer : ServerListener {
        private readonly RecvOps _receive;

        public LoginServer(int port) : base(port) {
            _receive = new RecvOps {
                [(short) ReceiveOperations.Login_OnCheckPasswordResult] = typeof(CheckPasswordEvent),
                [(short) ReceiveOperations.Login_OnWorldListReinitializeRequest] = typeof(WorldListEvent),
                [(short) ReceiveOperations.Login_OnChannelSelectEnterChannel] = typeof(SelectEnterChannelEvent),
                [(short) ReceiveOperations.Login_OnCheckUserLimitPacket] = typeof(CheckUserLimitEvent),
                [(short) ReceiveOperations.Login_OnLicenseResult] = typeof(LicenseResultEvent),
                [(short) ReceiveOperations.Login_OnSetGenderPacket] = typeof(SetGenderEvent),
                [(short) ReceiveOperations.Login_OnPinCodeResult] = typeof(PinCodeResultEvent),
                [(short) ReceiveOperations.Login_OnWorldListRequest] = typeof(WorldListEvent),
                [(short) ReceiveOperations.Login_OnViewAllCharPacket] = typeof(ViewAllCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnViewAllCharDlgResult] = typeof(ViewAllCharDlgEvent),
                [(short) ReceiveOperations.Login_OnCheckDuplicateIdPacket] = typeof(CheckDuplicateUsernameEvent),
                [(short) ReceiveOperations.Login_OnNewCharPacket22] = typeof(NewCharEvent),
                [(short) ReceiveOperations.Login_OnNewCharPacket23] = typeof(NewCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharInitSPWPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharSPWPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnSendBackupPacket] = typeof(BackupPacketEvent),
                [(short) ReceiveOperations.CWvsContext_OnUiMenu] = typeof(CWvsUiMenuEvent),
            };
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!_receive.Events.TryGetValue(operation, out Type t)) {
                Console.WriteLine($"[unhandled] {operation} (0x{operation:X2}) : {p.ToArrayString(true)}");
                Console.WriteLine($"[ascii-decode] {p}");
                Console.WriteLine("-----------");
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                try {
                    if (handler.OnProcess(p)) {
                        handler.OnHandle();
                    }
                } catch (Exception e) {
                    handler.OnError(e);
                }
            }
        }
    }
}