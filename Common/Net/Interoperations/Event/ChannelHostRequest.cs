using System.Linq;
using System.Net;
using log4net;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;

namespace NineToFive.Interopation.Event {
    public static class ChannelHostRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChannelHostRequest));

        public static byte[] OnHandle(Packet r) {
            IPAddress address = new IPAddress(r.ReadBytes(4));
            World world = Server.Worlds[r.ReadByte()];
            byte min = r.ReadByte(), max = r.ReadByte();
            Log.Info($"{address} is asking to host channels {min} - {max} in world {world.Id}");

            byte[] response = new byte[1];
            if (max >= world.Channels.Length) {
                // index out of bounds
                return response;
            }

            foreach (Channel channel in world.Channels.Where(ch => ch.HostAddress != null)) {
                // send a tiny packet to check the connection of the server
                if (!Interoperability.TestConnection(channel.HostAddress, channel.Port)) {
                    // test failed so assume server is offline
                    Log.Info($"Ping to channel {channel.Id} in world {world.Id} failed. It is now available for hosting");
                    channel.HostAddress = null;
                }
            }

            if (world.Channels.Any(ch => ch.HostAddress != null && ch.Id >= min && ch.Id <= max)) {
                // channel is already being hosted
                return response;
            }

            response[0] = 1; // granted permission
            for (int i = min; i <= max; i++) {
                Channel channel = world.Channels[i];
                ChannelSnapshot snapshot = channel.Snapshot;
                channel.HostAddress = address;
                Log.Info($"Channel {i} in world {channel.World.Id} is now hosted by {channel.HostAddress}");
            }

            return response;
        }
    }
}