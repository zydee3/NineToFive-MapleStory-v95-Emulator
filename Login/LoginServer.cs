using System;
using NineToFive.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;

namespace NineToFive.Login {
    class LoginServer : ServerListener {
        private RecvOps Receive { get; }

        public LoginServer(int port) : base(port) {
            Receive = new RecvOps();
            Receive.Events[(int) CLogin.OnSendBackupPacket] = typeof(BackupPacketEvent);
            Receive.Events[(int) CLogin.OnCheckPasswordResult] = typeof(CheckPasswordEvent);
            Receive.Events[(int) CLogin.OnLicenseResult] = typeof(LicenseResultEvent);
            Receive.Events[(int) CLogin.OnSetGenderPacket] = typeof(SetGenderEvent);
            Receive.Events[(int) CLogin.OnPinCodeResult] = typeof(PinCodeResultEvent);
            Receive.Events[(int) CLogin.OnWorldListRequest] = typeof(WorldListEvent);
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!Receive.Events.TryGetValue(operation, out Type t)) {
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