using NineToFive.Event;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Login.Event {
    public class ViewAllCharEvent : PacketEvent {
        public ViewAllCharEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            byte a = p.ReadByte();
            if (a == 1) {
                p.ReadString();
                byte[] machineId = p.ReadBytes(16); // CSystemInfo::GetMachineId
                p.ReadInt(); // CSystemInfo::GetGameRoomClient
                p.ReadByte();
                
                if (machineId.Length != Client.MachineId.Length) return false;
                for (byte i = 0; i < machineId.Length; i++) {
                    if (machineId[i] != Client.MachineId[i]) return false;
                }
            }
            
            return true;
        }

        public override void OnHandle() {
            Client.Session.Write(GetViewAllCharFail(1));
        }

        private static byte[] GetViewAllCharSuccess(Client client) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnViewAllCharResult);
            p.WriteByte(0);

            p.WriteByte();
            p.WriteByte((byte) client.Users.Count);
            foreach (User user in client.Users) {
                user.CharacterStat.Encode(user, p);
                user.AvatarLook.Encode(user, p);
                p.WriteBool(false); // rankings
            }
            return p.ToArray();
        }

        /// <summary>
        /// <para>1        for "Cannot find any characters.</para>
        /// <para>2        for "You are already connected to server.</para>
        /// <para>3,6,7    for "Unknown Error : View-All-Characters."</para>
        /// <para>default    for CLogin::RemoveNoticeConnecting</para>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static byte[] GetViewAllCharFail(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnViewAllCharResult);
            p.WriteByte(a);
            if (a == 1) {
                p.WriteInt();
                p.WriteInt();
            } else if (a == 3 || a == 6 || a == 7) {
                p.WriteBool(false);
            }

            return p.ToArray();
        }
    }
}