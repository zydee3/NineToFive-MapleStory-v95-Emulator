using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class MobPackets {
        private static void PoolInitMob(Mob mob, Packet w) {
            w.WriteShort((short) mob.Location.X);
            w.WriteShort((short) mob.Location.Y);
            w.WriteByte(mob.MoveAction);
            w.WriteShort((short) mob.Fh); // cur fh
            w.WriteShort((short) mob.Fh); // home fh
            w.WriteByte((byte) (mob.SummonType == 1 ? 254 : mob.SummonType));
            if (mob.SummonType == -3 || mob.SummonType >= 0) {
                w.WriteInt();
            }

            w.WriteByte(); // carnival team
            w.WriteInt((mob.HP / mob.MaxHP) * 100);
            w.WriteInt(); // nEffectItemID
        }

        private static void SetMobTemporaryStat(Mob mob, Packet w) {
            // CMob::SetTemporaryStat flags
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            // MobStat::DecodeTemporary
        }

        public static byte[] GetMobEnterField(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobEnterField);
            w.WriteUInt(mob.Id);
            w.WriteByte(5); // nCalcDamageIndex
            w.WriteInt(mob.TemplateId);

            SetMobTemporaryStat(mob, w);
            PoolInitMob(mob, w);

            return w.ToArray();
        }

        public static byte[] GetMobLeaveField(Mob mob, byte type) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobLeaveField);
            w.WriteUInt(mob.Id);
            byte b = w.WriteByte(type); // dead type
            if (b == 4) w.WriteInt();
            return w.ToArray();
        }

        public static byte[] GetMobChangeController(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobChangeController);

            // 1+ for CVecCtrlMob::SetMoveRandManSeed
            // 2 for CMob::ChaseTarget
            var controllerLevel = w.WriteByte((byte) (mob.ChaseTarget ? 2 : 1));
            w.WriteUInt(mob.Id);
            if (controllerLevel > 0) {
                w.WriteByte(5); // nCalcDamageIndex
                w.WriteInt(mob.TemplateId);
            }

            SetMobTemporaryStat(mob, w);
            // CMob::InitMob is only necessary if the controller is being set
            // and the client hasn't registered the mob (CMobPool::GetMob returns false) 
            PoolInitMob(mob, w);

            return w.ToArray();
        }

        public static byte[] GetShowHpIndicator(int mobId, byte health) {
            using Packet w = new Packet();
            w.WriteShort((short) CMob.OnHPIndicator);
            w.WriteInt(mobId);
            w.WriteByte(health);
            return w.ToArray();
        }
    }
}