using NineToFive.IO;
using System;

namespace NineToFive.Event {
    class CheckPasswordEvent : PacketEvent {
        private string Username, Password;
        private byte[] machineID;
        public CheckPasswordEvent(Client client) : base(client) {
        }

        public override void OnHandle() {
        }
        
        public override bool OnProcess(Packet packet) {
            Password = packet.ReadString();
            Username = packet.ReadString();
            machineID = packet.ReadBytes(16);
            packet.ReadInt(); // CSystemInfo::GetGameRoomClient
            packet.ReadByte(); // MEMORY[0x38]
            packet.ReadByte(); // 0
            packet.ReadByte(); // 0
            packet.ReadInt(); // partnerCode
            Client.Session.Write(GetLoginFailed(15));

            Console.WriteLine("Login attempt: \"{0}\" \"{1}\"", Username, Password);
            return true;
        }

        private static byte[] GetLoginSuccess() {
            Packet p = new Packet();
            p.WriteShort((short)SendOps.CLogin.OnCheckPasswordResult);
            return p.ToArray();
        }

        /// <summary>
        /// Represents a message popup image in the directory: <code>UI.wz/Login.img/Notice/text</code>
        /// </code>
        /// </summary>
        private static byte[] GetLoginFailed(byte b) {
            Packet p = new Packet();
            p.WriteShort((short)SendOps.CLogin.OnCheckPasswordResult);
            p.WriteByte(b);
            p.WriteByte(4);
            p.WriteInt();
            return p.ToArray();
        }
    }
}
