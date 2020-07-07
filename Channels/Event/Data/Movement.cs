using System.Numerics;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Event.Data {
    public class Movement : IPacketSerializer<Movement> {
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
        public byte SN { get; set; }
        public byte MoveAction { get; set; }

        public void Encode(Movement t, Packet p) {
            switch (t.Type) {
                case 0:
                case 5:
                case 12:
                case 14:
                case 35:
                case 36:
                    p.WriteShort((short) t.Location.X);
                    p.WriteShort((short) t.Location.Y);
                    p.WriteShort((short) t.Velocity.X);
                    p.WriteShort((short) t.Velocity.Y);
                    p.WriteShort(t.Fh);
                    if (t.Type == 12) p.WriteShort(t.FhFallStart);
                    p.WriteShort((short) t.Offset.X);
                    p.WriteShort((short) t.Offset.Y);
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
                    p.WriteShort((short) t.Velocity.X);
                    p.WriteShort((short) t.Velocity.Y);
                    break;
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                    p.WriteShort((short) t.Location.X);
                    p.WriteShort((short) t.Location.Y);
                    p.WriteShort(t.Fh);
                    break;
                case 9:
                    p.WriteByte(t.SN);
                    break;
                case 11:
                    p.WriteShort((short) t.Velocity.X);
                    p.WriteShort((short) t.Velocity.Y);
                    p.WriteShort(t.Fh);
                    break;
                case 17:
                    p.WriteShort((short)t.Location.X);
                    p.WriteShort((short)t.Location.Y);
                    p.WriteShort((short)t.Velocity.X);
                    p.WriteShort((short)t.Velocity.Y);
                    break;
            }
            p.WriteByte(MoveAction);
            p.WriteShort(Elapsed);
        }

        public void Decode(Movement t, Packet p) {
            switch (t.Type) {
                case 0:
                case 5:
                case 12:
                case 14:
                case 35:
                case 36:
                    t.Location = new Vector2(p.ReadShort(), p.ReadShort());
                    t.Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    t.Fh = p.ReadShort();
                    if (t.Type == 12) t.FhFallStart = p.ReadShort();
                    t.Offset = new Vector2(p.ReadShort(), p.ReadShort());
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
                    t.Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    break;
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                    t.Location = new Vector2(p.ReadShort(), p.ReadShort());
                    t.Fh = p.ReadShort();
                    break;
                case 9:
                    t.SN = p.ReadByte();
                    break;
                case 11:
                    t.Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    t.Fh = p.ReadShort();
                    break;
                case 17:
                    t.Location = new Vector2(p.ReadShort(), p.ReadShort());
                    t.Velocity = new Vector2(p.ReadShort(), p.ReadShort());
                    break;
            }
            MoveAction = p.ReadByte();
            Elapsed = p.ReadShort();
        }
    }
}