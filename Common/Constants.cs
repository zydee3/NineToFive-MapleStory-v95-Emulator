using System;
using System.Collections.Generic;

namespace NineToFive.Constants {
    public static class ServerConstants {
        public const int GameVersion = 95;

        public const string Address = "127.0.0.1";

        public const int InterCentralPort = 8481;
        public const int InterChannelPort = 8482;
        public const int InterLoginPort = 8483;

        public const int LoginPort = 8484;
        public const int ChannelPort = 7575;

        public const int WorldCount = 1;
        public const int ChannelCount = 3;
        public const bool EnabledRanking = false;

        public static readonly string[] WorldNames = {
            "Scania", "Bera", "Broa", "Windia", "Khaini", "Bellocan", "Mardia", "kradia", "Yellonde", "Demethos",
            "Galicia", "El Nido (East)", "Zenith", "Arcania", "Plana", "Kastia", "Kalluna", "Stius", "Croa", "Judis",
            "Nova", "Aster", "Cosmo", "Androa"
        };
    }

    public static class Disabled {
        public static readonly List<uint> Mobs = new List<uint> { };
        public static readonly List<uint> Npcs = new List<uint> { };
        public static readonly List<uint> Cash = new List<uint> { };
        public static readonly List<uint> Fields = new List<uint> { };
    }

    public static class Job {
        public static bool IsExtendedSpJob(int jobId) => !(jobId / 1000 != 3 && jobId / 100 != 22 && jobId != 2001);
    }

    public static class ItemConstants {
        public static byte GetGenderFromId(int itemId) {
            if (itemId / 1000000 != 1) return 2;
            return (byte) Math.Min(2, itemId / 1000 % 10);
        }

        public static short GetBodyPartFromId(int itemId) {
            int category = itemId / 10000;
            switch (category) {
                case 100: return 1;
                case 101: return 2;
                case 102: return 3;
                case 103: return 4;
                case 104:
                case 105: return 5;
                case 106: return 6;
                case 107: return 7;
                case 108: return 8;
                case 109:
                case 119:
                case 134: return 10;
                case 110: return 9;
                case 111:
                    // rings can be 12, 13, 15 or 16
                    return 12;
                case 112: // seems to be 17 or 59
                    return 17;
                case 113: return 50;
                case 114: return 49;
                case 115: return 51;
                case 161: return 1100;
                case 162: return 1101;
                case 163: return 1102;
                case 164: return 1103;
                case 165: return 1104;
                case 180 when itemId == 1802100:
                    // can be 21, 31, 39
                    return 21;
                case 180:
                    // can be 14, 30, 38
                    return 14;
                case 181:
                case 182:
                    switch (itemId) {
                        case 1812000:
                            // can be 23, 34, 42
                            return 23;
                        case 1812001:
                            // can be 22, 33, 41
                            return 22;
                        case 1812002: return 24;
                        case 1812003: return 25;
                        case 1812004:
                            // can  be 26, 35, 43
                            return 26;
                        case 1812005:
                            // can be 27, 36, 44
                            return 27;
                        case 1812006:
                            // can be 28, 37, 45
                            return 28;
                        case 1812007:
                            // can be 46, 47, 48
                            return 46;
                    }

                    // can be 21, 31, 39
                    return 21;
                case 183:
                    // can be 29, 32, 40
                    return 29;
                case 190: return 18;
                case 191: return 19;
                case 192: return 20;
                case 194: return 1000;
                case 195: return 1001;
                case 196: return 1002;
                case 197: return 1003;
                default:
                    int sub = category / 10;
                    if (sub != 13 && sub != 14 && sub != 16 && sub != 17)
                        break;
                    return 11;
            }

            throw new InvalidOperationException($"could not get body part for item {itemId}, category {category}");
        }

        public static bool IsCorrectBodyPart(int itemId, int bagIndex, byte gender) {
            byte reqGender = GetGenderFromId(itemId);
            if (gender != 2 && reqGender != 2 && reqGender != gender) return false;
            int category = itemId / 10000;
            switch (category) {
                case 100: return bagIndex == 1;
                case 101: return bagIndex == 2;
                case 102: return bagIndex == 3;
                case 103: return bagIndex == 4;
                case 104:
                case 105: return bagIndex == 5;
                case 106: return bagIndex == 6;
                case 107: return bagIndex == 7;
                case 108: return bagIndex == 8;
                case 109:
                case 119:
                case 134: return bagIndex == 10;
                case 110: return bagIndex == 9;
                case 111: return bagIndex == 12 || bagIndex == 13 || bagIndex == 15 || bagIndex == 16;
                case 112: return bagIndex == 17 || bagIndex == 59;
                case 113: return bagIndex == 50;
                case 114: return bagIndex == 49;
                case 115: return bagIndex == 51;
                case 161: return bagIndex == 1100;
                case 162: return bagIndex == 1101;
                case 163: return bagIndex == 1102;
                case 164: return bagIndex == 1103;
                case 165: return bagIndex == 1104;
                case 180:
                    if (itemId == 1802100) return bagIndex == 21 || bagIndex == 31 || bagIndex == 39;
                    return bagIndex == 14 || bagIndex == 30 || bagIndex == 38;
                case 181:
                    switch (itemId) {
                        case 1812000: return bagIndex == 23 || bagIndex == 34 || bagIndex == 42;
                        case 1812001: return bagIndex == 22 || bagIndex == 33 || bagIndex == 41;
                        case 1812002: return bagIndex == 24;
                        case 1812003: return bagIndex == 25;
                        case 1812004: return bagIndex == 26 || bagIndex == 35 || bagIndex == 43;
                        case 1812005: return bagIndex == 27 || bagIndex == 36 || bagIndex == 44;
                        case 1812006: return bagIndex == 28 || bagIndex == 37 || bagIndex == 45;
                        case 1812007: return bagIndex == 46 || bagIndex == 47 || bagIndex == 48;
                        default:      return false;
                    }
                case 182: return bagIndex == 21 || bagIndex == 31 || bagIndex == 39;
                case 183: return bagIndex == 29 || bagIndex == 32 || bagIndex == 40;
                case 190: return bagIndex == 18;
                case 191: return bagIndex == 19;
                case 192: return bagIndex == 20;
                case 194: return bagIndex == 1000;
                case 195: return bagIndex == 1001;
                case 196: return bagIndex == 1002;
                case 197: return bagIndex == 1003;
                default:
                    int sub = category / 10;
                    if (sub != 13 && sub != 14 && sub != 16 && sub != 17) return false;
                    return bagIndex == 11;
            }
        }
    }

    public enum EntityType {
        Npc,
        Mob,
        Player,
        Reactor,
        Summon
    }

    public enum TemplateType {
        Mob,
        Field
    }

    public enum SkillProperties {
        acc,
        asrR,
        attackCount,
        bulletCount,
        cooltime,
        cr,
        criticaldamageMax,
        criticaldamageMin,
        damage,
        damR,
        dot,
        dotInterval,
        dotTime,
        emdd,
        epad,
        epdd,
        er,
        eva,
        expR,
        hpCon,
        ignoreMobpdpR,
        jump,
        mad,
        mastery,
        maxLevel,
        mdd,
        mhpR,
        mobCount,
        morph,
        mp,
        mpCon,
        pad,
        padX,
        pdd,
        pddR,
        prop,
        range,
        speed,
        subProp,
        subTime,
        t,
        terR,
        time,
        u,
        v,
        w,
        x,
        y,
        z,
        selfDestruction,
        itemCon,
        itemConNo,
        bulletConsume,
        emmp,
        emhp,
        action,
        mesoR,
        madX,
        mmpR,
        hp,
        moneyCon,
        itemConsume,
        mddR
    }

    public enum FieldLimits {
        CanJump,
        CanUseSkill,
        CanUseSummonItem,
        CanUseMysticDoor,
        CanMigrate,
        CanUsePortalScroll,
        CanUseTeleportItem,
        CanOpenMiniGame,
        CanUseSpecificPortalScroll,
        CanUseTamingMob,
        CanConsumeStatChangeItem,
        CanChangePartyBoss,
        HasMonsterCapacityLimit,
        CanUseWeddingInvitationItem,
        CanUseCashWeather,
        CanUsePet,
        CanUseAntiMacroItem,
        CanFallDown,
        CanSummonNPC,
        HasExpDecrease,
        HasDamageOnFall,
        HasParacelOpenLimit,
        HasDropLimit,
        CanUseRocketBoost,
        HasItemOptionLimit,
        HasQuestAlert,
        CanUseAndroid,
        AutoExpandMiniMap,
        CanUseNonMoveSkill,
        DisableMiniMap,
        HasSwim
    }
}