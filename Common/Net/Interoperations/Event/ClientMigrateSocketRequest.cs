using log4net;
using NineToFive.Game;

namespace NineToFive.Net.Interoperations.Event {
    public static class ClientMigrateSocketRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientMigrateSocketRequest));

        public static byte[] OnHandle(Packet r) {
            World world = Server.Worlds[r.ReadByte()];
            Channel channel = world.Channels[r.ReadByte()];

            using Packet w = new Packet();
            // check if server end-point is unavailable
            bool alive = channel.HostAddress != null && Interoperability.TestConnection(channel.HostAddress, channel.Port);
            w.WriteBool(alive);
            if (alive) {
                w.WriteBytes(channel.HostAddress.GetAddressBytes());
            } else {
                channel.HostAddress = null;
                Log.Info($"No server is hosting channel {channel.Id} in world {world.Id}");
            }

            return w.ToArray();
        }
    }
}