using System.Linq;
using NineToFive.Game;
using NineToFive.IO;

namespace NineToFive.Interopation.Event {
    public static class WorldInformationRequest {
        public static byte[] OnHandle() {
            using Packet w = new Packet();
            foreach (World world in Server.Worlds) {
                w.WriteByte(world.Id);
                // find only channels that have a listening socket on this server
                foreach (Channel channel in world.Channels.Where(ch => ch.ServerListener != null)) {
                    w.WriteByte(channel.Id);
                    w.WriteInt(channel.Snapshot.UserCount);
                }

                w.WriteByte(255);
            }

            w.WriteByte(255);

            return w.ToArray();
        }
    }
}