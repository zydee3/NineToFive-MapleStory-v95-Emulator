using System.Collections.Generic;
using System.Numerics;
using NineToFive.Event.Data;
using NineToFive.Net;

namespace NineToFive.Packets {
    public static class MovePath {
        public static void Encode(Packet w, Vector2 origin, Vector2 velocity, List<Movement> moves) {
            w.WriteShort((short) origin.X);
            w.WriteShort((short) origin.Y);
            w.WriteShort((short) velocity.X);
            w.WriteShort((short) velocity.Y);
            w.WriteByte((byte) moves.Count);
            foreach (var move in moves) {
                move.Encode(w);
            }

            for (int i = 0; i < w.WriteByte(); i++) {
                w.WriteByte();
            }

            w.WriteShort();
            w.WriteShort();
            w.WriteShort();
            w.WriteShort();
        }
    }
}