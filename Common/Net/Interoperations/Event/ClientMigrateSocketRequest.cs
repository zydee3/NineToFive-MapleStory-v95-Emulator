using System.Net;
using log4net;
using NineToFive.Game;

namespace NineToFive.Net.Interoperations.Event {
    public static class ClientMigrateSocketRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientMigrateSocketRequest));

        public static byte[] OnHandle(Packet r) {
            Client client = Server.AddClientIfAbsent(r.ReadString());
            client.LastKnownIp = new IPAddress(r.ReadBytes(4));

            World world = Server.Worlds[r.ReadByte()];
            Channel channel = world.Channels[r.ReadByte()];

            using Packet w = new Packet();
            if (w.WriteBool(channel.HostAddress != null)) {
                Log.Info($"Channel address found {channel.HostAddress}");
                w.WriteBytes(channel.HostAddress.GetAddressBytes());
            } else {
                Log.Info($"No server is hosting channel {channel.Id} in world {world.Id}");
            }

            return w.ToArray();
        }
    }
}