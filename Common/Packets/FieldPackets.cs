using System.Numerics;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class FieldPackets {
        /// <summary>
        /// <code>1 for     "The portal is closed for now."</code>
        /// <code>2 for     "You cannot go to that place."</code>
        /// <code>3 for     "Unable to approach due the the force of the ground."</code>
        /// <code>4 for     "You cannot teleport to or on this map"</code>
        /// <code>5 for     "Unable to approach due to the force of the ground."</code>
        /// <code>6 for     "This map can only be entered by party members."</code>
        /// <code>7 for     "Only members of an expedition can enter this map."</code>
        /// <code>8 for     "The Cash Shop is currently not available. Stay Tuned."</code>
        /// </summary>
        /// <param name="message">message type to be displayed</param>
        public static byte[] GetTransferFieldRequestIgnored(byte message) {
            using Packet w = new Packet();
            w.WriteShort((short) CField.OnTransferFieldReqIgnored);
            w.WriteByte(message);
            return w.ToArray();
        }
    }
}