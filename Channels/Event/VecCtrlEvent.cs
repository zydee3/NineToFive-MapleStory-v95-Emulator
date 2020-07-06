using System.Collections.Generic;
using System.Numerics;
using NineToFive.Event.Data;
using NineToFive.Net;

namespace NineToFive.Event {
    public class VecCtrlEvent : PacketEvent {
        public List<Movement> Movements = new List<Movement>();
        public VecCtrlEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt();
            p.ReadInt();
            p.ReadByte();
            p.ReadInt();
            p.ReadInt();
            
            Origin = new Vector2(p.ReadShort(), p.ReadShort());
            Velocity = new Vector2(p.ReadShort(), p.ReadShort());
            byte count = p.ReadByte();
            while (count-- > 0) {
                byte type = p.ReadByte();
                Movement move = new Movement(type);
                move.Decode(move, p);
                Movements.Add(move);
            }

            return false;
        }

        public Vector2 Origin { get; set; }
        public Vector2 Velocity { get; set; }
    }
}