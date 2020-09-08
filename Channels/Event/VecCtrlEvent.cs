using System.Collections.Generic;
using System.Numerics;
using NineToFive.Event.Data;
using NineToFive.Game.Entity;
using NineToFive.Net;

namespace NineToFive.Event {
    public class VecCtrlEvent : PacketEvent {
        public List<Movement> Movements;
        public VecCtrlEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            var user = Client.User;

            Origin = new Vector2(p.ReadShort(), p.ReadShort());
            Velocity = new Vector2(p.ReadShort(), p.ReadShort());
            byte count = p.ReadByte();
            Movements = new List<Movement>(count);
            while (count-- > 0) {
                byte type = p.ReadByte();
                Movement move = new Movement(type);
                move.Decode(p);
                Movements.Add(move);
            }

            return Movements.Count > 0;
        }

        public void ApplyLatestMovement(Life life) {
            var movement = Movements[^1];
            life.Location = movement.Location;
            life.Velocity = movement.Velocity;
            life.MoveAction = movement.MoveAction;
            life.Fh = movement.Fh;
        }

        public Vector2 Origin { get; set; }
        public Vector2 Velocity { get; set; }
    }
}