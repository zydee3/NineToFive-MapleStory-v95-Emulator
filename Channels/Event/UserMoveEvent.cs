using System.Collections.Generic;
using System.Numerics;
using NineToFive.Event.Data;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class UserMoveEvent : VecCtrlEvent {
        public UserMoveEvent(Client client) : base(client) { }

        public override void OnHandle() {
            var user = Client.User;
            user.Field.BroadcastSkip(GetUserRemoteMove(user, Origin, Velocity, Movements), Client.Id);
        }

        private static byte[] GetUserRemoteMove(User user, Vector2 origin, Vector2 velocity, List<Movement> moves) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserRemote.OnMove);
            w.WriteShort((short) origin.X);
            w.WriteShort((short) origin.Y);
            w.WriteShort((short) velocity.X);
            w.WriteShort((short) velocity.Y);
            w.WriteSByte((sbyte) moves.Count);
            foreach (var move in moves) {
                move.Encode(move, w);
            }

            for (int i = 0; i < w.WriteByte(); i++) {
                w.WriteByte();
            }

            w.WriteShort();
            w.WriteShort();
            w.WriteShort();
            w.WriteShort();
            return w.ToArray();
        }
    }
}