using NineToFive.Event;
using NineToFive.Game;
using NineToFive.IO;

namespace NineToFive.Login.Event {
    public class CheckUserLimitEvent : PacketEvent {
        private short _worldId;
        
        public CheckUserLimitEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _worldId = p.ReadShort();
            return _worldId >= 0 && _worldId < Server.Worlds.Length;
        }

        public override void OnHandle() {
            World world = Server.Worlds[_worldId];
            Client.Session.Write(GetCheckUserLimitResult((byte) (world.Users.Count > 60 ? 1 : 0)));
        }

        /// <summary>
        /// <para>0    for CUIChannelSelect::CUIChannelSelect</para>
        /// <para>1    for "Since There Are Many\r\nConcurrent Users in This World,\r\nYou May Encounter Some Difficulties During the Game Play."</para>
        /// <para>2    for "The Concurrent Users in This World\r\nHave Reached the Max.\r\nPlease Try Again Later."</para>
        /// </summary>
        /// <param name="a">result of user limit check</param>
        /// <returns></returns>
        private static byte[] GetCheckUserLimitResult(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) SendOps.CLogin.OnCheckUserLimitResult);
            p.WriteByte(a);
            p.WriteByte();
            return p.ToArray();
        }
    }
}