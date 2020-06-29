using System;
using NineToFive.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;

namespace NineToFive.Login {
    class LoginServer : ServerListener {
        private readonly RecvOps _receive;

        public LoginServer(int port) : base(port) {
            _receive = new RecvOps {
                [(short) CLogin.OnSendBackupPacket] = typeof(BackupPacketEvent),
                [(short) CLogin.OnCheckPasswordResult] = typeof(CheckPasswordEvent),
                [(short) CLogin.OnChannelSelectEnterChannel] = typeof(SelectEnterChannelEvent),
                [(short) CLogin.OnCheckUserLimitPacket] = typeof(CheckUserLimitEvent),
                [(short) CLogin.OnLicenseResult] = typeof(LicenseResultEvent),
                [(short) CLogin.OnSetGenderPacket] = typeof(SetGenderEvent),
                [(short) CLogin.OnPinCodeResult] = typeof(PinCodeResultEvent),
                [(short) CLogin.OnWorldListRequest] = typeof(WorldListEvent),
                [(short) CLogin.OnViewAllCharPacket] = typeof(ViewAllCharEvent),
                [(short) CLogin.OnCheckDuplicateIdPacket] = typeof(CheckDuplicateUsernameEvent),
                [(short) CLogin.OnViewAllCharDlgResult] = typeof(ViewAllCharDlgEvent),
                
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
                    Console.WriteLine($"[handled] {handler.GetType().Name}");
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