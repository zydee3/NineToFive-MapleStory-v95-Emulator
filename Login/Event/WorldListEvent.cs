using NineToFive.Game;
using NineToFive.IO;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class WorldListEvent : PacketEvent {
        public WorldListEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() {
            foreach (World world in Server.Worlds) {
                Client.Session.Write(GetWorldInformation(world));
            }

            Client.Session.Write(GetWorldSelect());
        }

        private static byte[] GetWorldInformation(World world) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnWorldInformation);
            p.WriteByte(world.Id);

            p.WriteString(world.Name);
            p.WriteByte(); // flag
            p.WriteString(); // message

            p.WriteShort();
            p.WriteShort();
            p.WriteByte();
            p.WriteByte((byte) world.Channels.Length);
            foreach (Channel channel in world.Channels) {
                p.WriteString($"{world.Name}-{channel.Id + 1}");
                p.WriteInt(); // load
                p.WriteByte(); // unknown
                p.WriteByte(); // world_id
                p.WriteByte(); // channel_id
            }

            p.WriteShort(); // CLogin::BALLOON
            return p.ToArray();
        }

        private static byte[] GetWorldSelect() {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnWorldInformation);
            p.WriteSByte(-1);
            return p.ToArray();
        }
    }
}