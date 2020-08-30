using System;
using NineToFive.Net;

namespace NineToFive.Game.Entity.Meta {
    public static class ForcedStatExt {
        public static void SetValue(this ForcedStatType type, ForcedStat s, short v) {
            switch (type) {
                case ForcedStatType.Str:
                    s.Str = v;
                    break;
                case ForcedStatType.Dex:
                    s.Dex = v;
                    break;
                case ForcedStatType.Int:
                    s.Int = v;
                    break;
                case ForcedStatType.Luk:
                    s.Luk = v;
                    break;
                case ForcedStatType.PAD:
                    s.PAD = v;
                    break;
                case ForcedStatType.PDD:
                    s.PDD = v;
                    break;
                case ForcedStatType.MAD:
                    s.MAD = v;
                    break;
                case ForcedStatType.MDD:
                    s.MDD = v;
                    break;
                case ForcedStatType.Acc:
                    s.Acc = v;
                    break;
                case ForcedStatType.Eva:
                    s.Eva = (byte) v;
                    break;
                case ForcedStatType.Speed:
                    s.Speed = (byte) v;
                    break;
                case ForcedStatType.Jump:
                    s.Jump = (byte) v;
                    break;
                case ForcedStatType.SpeedMax:
                    s.SpeedMax = (byte) v;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void EncodeType(this ForcedStatType type, User user, Packet w) {
            switch (type) {
                case ForcedStatType.Str:
                    w.WriteShort(user.ForcedStat.Str);
                    break;
                case ForcedStatType.Dex:
                    w.WriteShort(user.ForcedStat.Dex);
                    break;
                case ForcedStatType.Int:
                    w.WriteShort(user.ForcedStat.Int);
                    break;
                case ForcedStatType.Luk:
                    w.WriteShort(user.ForcedStat.Luk);
                    break;
                case ForcedStatType.PAD:
                    w.WriteShort(user.ForcedStat.PAD);
                    break;
                case ForcedStatType.PDD:
                    w.WriteShort(user.ForcedStat.PDD);
                    break;
                case ForcedStatType.MAD:
                    w.WriteShort(user.ForcedStat.MAD);
                    break;
                case ForcedStatType.MDD:
                    w.WriteShort(user.ForcedStat.MDD);
                    break;
                case ForcedStatType.Acc:
                    w.WriteShort(user.ForcedStat.Acc);
                    break;
                case ForcedStatType.Eva:
                    w.WriteByte(user.ForcedStat.Eva);
                    break;
                case ForcedStatType.Speed:
                    w.WriteByte(user.ForcedStat.Speed);
                    break;
                case ForcedStatType.Jump:
                    w.WriteByte(user.ForcedStat.Jump);
                    break;
                case ForcedStatType.SpeedMax:
                    w.WriteByte(user.ForcedStat.SpeedMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public class ForcedStat {
        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public short PAD { get; set; }
        public short PDD { get; set; }
        public short MAD { get; set; }
        public short MDD { get; set; }
        public short Acc { get; set; }
        public byte Eva { get; set; }
        public byte Speed { get; set; }
        public byte Jump { get; set; }
        public byte SpeedMax { get; set; }

        public void SetByFlag(uint flag, short value) {
            ((ForcedStatType) flag).SetValue(this, value);
        }
    }

    [Flags]
    public enum ForcedStatType {
        Str = 1,
        Dex = 2,
        Int = 4,
        Luk = 8,
        PAD = 16,
        PDD = 32,
        MAD = 64,
        MDD = 128,
        Acc = 256,
        Eva = 512,
        Speed = 1024,
        Jump = 2048,
        SpeedMax = 4096,
    }
}