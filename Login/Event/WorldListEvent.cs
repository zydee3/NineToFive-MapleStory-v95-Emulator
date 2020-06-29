using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class WorldListEvent : PacketEvent {
        public WorldListEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() {
            RequestWorldInformation();
            foreach (World world in Server.Worlds) {
                Client.Session.Write(GetWorldInformation(world));
            }

            Client.Session.Write(GetWorldSelect());
        }

        private static void RequestWorldInformation() {
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperations.WorldInformationRequest);
            byte[] response = Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort);
            using Packet r = new Packet(response);
            for (int worldId = r.ReadByte(); worldId != 255; worldId = r.ReadByte()) {
                World world = Server.Worlds[worldId];
                for (int channelId = r.ReadByte(); channelId != 255; channelId = r.ReadByte()) {
                    world.Channels[channelId].Snapshot.UserCount = r.ReadInt();
                }
            }
        }

        private static byte[] GetWorldInformation(World world) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnWorldInformation);
            p.WriteByte(world.Id);

            p.WriteString(world.Name);
            p.WriteByte();   // flag
            p.WriteString(); // message

            p.WriteShort();
            p.WriteShort();
            p.WriteByte();
            p.WriteByte((byte) world.Channels.Length);
            foreach (Channel channel in world.Channels) {
                p.WriteString($"{world.Name}-{channel.Id + 1}");
                p.WriteInt();  // load, 1000 = 100% population
                p.WriteByte(); // unknown
                p.WriteByte();
                p.WriteByte();
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