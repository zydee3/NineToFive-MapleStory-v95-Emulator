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
            var latest = Movements[^1];
            var user = Client.User;
            user.Location = latest.Location;
            user.Field.BroadcastPacketExclude(user, GetUserRemoteMove(user, Origin, Velocity, Movements));
        }

        private static byte[] GetUserRemoteMove(User user, Vector2 origin, Vector2 velocity, List<Movement> moves) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserRemote.OnMove);
            w.WriteShort((short) origin.X);
            w.WriteShort((short) origin.Y);
            w.WriteShort((short) velocity.X);
            w.WriteShort((short) velocity.Y);
            w.WriteByte((byte) moves.Count);
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