using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class DropPool {
        /// <summary>
        /// <code>0    drops from Drop.Origin, disappears when landing</code>
        /// <code>1    drops from Drop.Origin, lands at Drop.Location</code>
        /// <code>2    no animation, instantly appears at Drop.Location</code>
        /// <code>3    drops from Drop.Origin, disappears when falling</code>
        /// <code>4    static image, slowly lands at Drop.Location</code>
        /// </summary>
        /// <param name="drop">drop object</param>
        /// <param name="type">animation type for the drop</param>
        /// <returns></returns>
        public static byte[] GetDropEnterField(Drop drop, byte type) {
            using Packet w = new Packet();
            w.WriteShort((short) CDropPool.OnDropEnterField);
            w.WriteByte(type);
            w.WriteUInt(drop.Id);

            byte a = w.WriteByte();
            w.WriteInt(drop.TemplateId);
            w.WriteInt();
            w.WriteByte((byte) drop.Fh);
            w.WriteShort((short) drop.Location.X); // destination
            w.WriteShort((short) drop.Location.Y);
            w.WriteInt();

            if (type == 0 || type == 1 || type == 3 || type == 4) {
                w.WriteShort((short) drop.Origin.X);
                w.WriteShort((short) drop.Origin.Y);
                w.WriteShort();
            }

            if (a == 0) {
                w.WriteLong();
            }

            w.WriteByte();
            w.WriteBool(false);
            return w.ToArray();
        }

        public static byte[] GetDropLeaveField(Drop drop, byte type, int pickupId = 0) {
            using Packet w = new Packet();
            w.WriteShort((short) CDropPool.OnDropLeaveField);
            w.WriteByte(type);
            w.WriteUInt(drop.Id);
            
            if (type == 2 || type == 3 || type == 5) w.WriteInt(pickupId);
            else if (type == 4) w.WriteShort();
            if (type == 5) w.WriteInt();

            return w.ToArray();
        }
    }
}