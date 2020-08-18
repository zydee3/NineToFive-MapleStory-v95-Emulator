using System;

namespace NineToFive.Game.Entity.Meta {
    [Flags]
    public enum TemporaryStat {
        None = 0,
        PAD = 1,  // PhysicalDamage
        PDD = 2,  // PhysicalDefense
        MAD = 4,  // MagicDamage
        MDD = 8,  // MagicDefense
        Acc = 16, // Accuracy
        Eva = 32, // Evasion
        Hands = 64,
        Speed = 128,
        Jump = 256,
        Ghost = 512,
    }
}