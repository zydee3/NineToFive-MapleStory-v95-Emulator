using System;
using NineToFive.Game;

namespace NineToFive.Net.Interoperations.Event {
    public static class ChannelUserLimitRequest {
        public static byte[] OnHandle(Packet r) {
            World world = Server.Worlds[r.ReadByte()];
            Channel channel = world.Channels[r.ReadByte()];
            DateTime now = DateTime.Now;
            TimeSpan diff = now - channel.Snapshot.Timestamp;

            // request user count from the specified channel server
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.ChannelUserLimitResponse);
            w.WriteByte(world.Id);
            w.WriteByte(channel.Id);
            if (channel.HostAddress != null) {
                byte[] userCount = BitConverter.GetBytes(channel.Snapshot.UserCount);
                if (diff >= TimeSpan.FromMinutes(10)) {
                    userCount = Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterChannelPort, channel.HostAddress.ToString());
                    channel.Snapshot.Timestamp = now;
                    channel.Snapshot.UserCount = BitConverter.ToInt32(userCount);
                    // Log.Info($"Updating user count for channel {_channelId} in world {_worldId}");
                }

                w.WriteBytes(userCount);
            } else {
                // channel server not online?
                w.WriteInt();
            }

            return w.ToArray();
        }
    }
}