using System.Collections.Generic;
using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event.Data {
    public class MobGenerateMovePathEvent : VecCtrlEvent {
        private uint _mobId;
        private short _mobCtrlSn;

        public MobGenerateMovePathEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _mobId = p.ReadUInt();      // dwMobID
            _mobCtrlSn = p.ReadShort(); // m_nMobCtrlSN
            int a = p.ReadByte();       // v171 | 4 * (v168 | 2 * (a10 | 2 * v61))
            int b = p.ReadByte();
            int c = p.ReadInt();

            for (int i = 0; i < p.ReadInt(); i++) {
                p.ReadInt();
                p.ReadInt();
            }

            for (int i = 0; i < p.ReadInt(); i++) {
                p.ReadInt();
            }

            int d = p.ReadByte();
            int e = p.ReadInt();
            int f = p.ReadInt();
            int g = p.ReadInt();
            int h = p.ReadInt();
            bool result = base.OnProcess(p);
            int ii = p.ReadByte();
            int j = p.ReadByte();
            int k = p.ReadByte();
            int l = p.ReadByte();
            int m = p.ReadInt();

            return true;
        }

        public override void OnHandle() {
            var user = Client.User;
            var mob = user.Field.LifePools[EntityType.Mob][_mobId] as Mob;
            if (mob == null || user.Field != mob.Field) return;
            ApplyLatestMovement(mob);
            // tell everyone EXCLUDING the controller owner that the mob has moved
            // including the controller owner will cause mobs to become inactive
            user.Field.BroadcastPacketExclude(user, GetMobMove(mob, Origin, Velocity, Movements));
            user.Client.Session.Write(GetMobCtrlAck(mob)); // only the controller owner should receive this
        }

        /// <summary>
        /// let the controller owner know the movement packet has been handled (Mob Controller Acknowledge) 
        /// </summary>
        /// <param name="mob">the mob that is moving</param>
        private byte[] GetMobCtrlAck(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMob.OnCtrlAck);
            w.WriteUInt(mob.Id);

            w.WriteShort(_mobCtrlSn); // nMobCtrlSN
            w.WriteByte();            // bNextAttackPossible
            w.WriteShort();           // m_nMP
            w.WriteByte();            // m_nUserCtrlCommand
            w.WriteByte();            // nSLV ( skill level )

            return w.ToArray();
        }

        private static byte[] GetMobMove(Mob mob, Vector2 origin, Vector2 velocity, List<Movement> moves) {
            using Packet w = new Packet();
            w.WriteShort((short) CMob.OnMove);
            w.WriteUInt(mob.Id);

            w.WriteByte(); // bNotForceLanding
            w.WriteByte();
            byte v7 = w.WriteByte();
            byte v1 = w.WriteByte(); // nActionAndDir
            if (v7 > 0 || ((v1 >> 1) - 13) <= 8) { }

            w.WriteInt();
            for (int i = 0; i < w.WriteInt(); i++) {
                w.WriteInt();
                w.WriteInt();
            }

            for (int i = 0; i < w.WriteInt(); i++) {
                w.WriteInt();
            }

            MovePath.Encode(w, origin, velocity, moves);
            return w.ToArray();
        }
    }
}