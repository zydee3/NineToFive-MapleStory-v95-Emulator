using System.Numerics;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Event.Data {
    public class Movement : IPacketSerializer {
        public byte Type { get; }

        public Movement(in byte type) {
            Type = type;
        }

        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Offset { get; set; }
        public short Fh { get; set; }
        public short FhFallStart { get; set; }
        public short Elapsed { get; set; }
        public byte Sn { get; set; }
        public byte MoveAction { get; set; }

        public void Encode(Packet p) {
            switch (p.WriteByte(Type)) {
                case 0:
                case 5:
                case 12:
                case 14:
                case 35:
                case 36:
                    p.WriteShort((short) Location.X);
                    p.WriteShort((short) Location.Y);
                    p.WriteShort((short) Velocity.X);
                    p.WriteShort((short) Velocity.Y);
                    p.WriteShort(Fh);
                    if (Type == 12) p.WriteShort(FhFallStart);
                    p.WriteShort((short) Offset.X);
                    p.WriteShort((short) Offset.Y);
                    break;
                case 1:
                case 2:
                case 13:
                case 16:
                case 18:
                case 31:
                case 32:
                case 33:
                case 34:
                    p.WriteShort((short) Velocity.X);
                    p.WriteShort((short) Velocity.Y);
                    break;
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                    p.WriteShort((short) Location.X);
                    p.WriteShort((short) Location.Y);
                    p.WriteShort(Fh);
                    break;
                case 9:
                    p.WriteByte(Sn);
                    break;
                case 11:
                    p.WriteShort((short) Velocity.X);
                    p.WriteShort((short) Velocity.Y);
                    p.WriteShort(Fh);
                    break;
                case 17:
                    p.WriteShort((short) Location.X);
                    p.WriteShort((short) Location.Y);
                    p.WriteShort((short) Velocity.X);
                    p.WriteShort((short) Velocity.Y);
                    break;
            }
            p.WriteByte(MoveAction);
            p.WriteShort(Elapsed);
        }

        public void Decode(Packet p) {
            switch (Type) {
                case 0:
                case 5:
                case 12:
                case 14:
                case 35:
                case 36:
                    Location = new Vector2(p.ReadShort(), p.ReadShort());
                    Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    Fh = p.ReadShort();
                    if (Type == 12) FhFallStart = p.ReadShort();
                    Offset = new Vector2(p.ReadShort(), p.ReadShort());
                    break;
                case 1:
                case 2:
                case 13:
                case 16:
                case 18:
                case 31:
                case 32:
                case 33:
                case 34:
                    Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    break;
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                    Location = new Vector2(p.ReadShort(), p.ReadShort());
                    Fh = p.ReadShort();
                    break;
                case 9:
                    Sn = p.ReadByte();
                    break;
                case 11:
                    Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    Fh = p.ReadShort();
                    break;
                case 17:
                    Location = new Vector2(p.ReadShort(), p.ReadShort());
                    Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    break;
            }
            MoveAction = p.ReadByte();
            Elapsed = p.ReadShort();
        }
    }
}