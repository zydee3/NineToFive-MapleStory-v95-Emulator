using System.Collections.Generic;
using System.Numerics;
using NineToFive.Event.Data;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class UserMoveEvent : VecCtrlEvent {
        public UserMoveEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.Position += 29;
            return base.OnProcess(p);
        }

        public override void OnHandle() {
            var user = Client.User;
            ApplyLatestMovement(user);
            user.Field.BroadcastPacketExclude(user, GetUserRemoteMove(user.CharacterStat.Id, Origin, Velocity, Movements));
        }

        private static byte[] GetUserRemoteMove(uint characterId, Vector2 origin, Vector2 velocity, List<Movement> moves) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserRemote.OnMove);
            w.WriteUInt(characterId);
            MovePath.Encode(w, origin, velocity, moves);

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