using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;

namespace NineToFive.Interopation.Event {
    public static class ChannelUserLimitRequest {
        public static byte[] OnHandle(Packet r) {
            World world = Server.Worlds[r.ReadByte()];
            Channel channel = world.Channels[r.ReadByte()];
            // request user count from the specified channel server
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.ChannelUserLimitResponse);
            w.WriteByte(world.Id);
            w.WriteByte(channel.Id);
            if (channel.HostAddress != null) {
                w.WriteBytes(Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterChannelPort, channel.HostAddress.ToString()));
            } else {
                // channel server not online?
                w.WriteInt();
            }

            return w.ToArray();
        }
    }
}