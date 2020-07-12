using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using IniParser;
using IniParser.Model;
using log4net;
using NineToFive.Game.Storage;

namespace NineToFive {
    public static class ServerConstants {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerConstants));
        private const string ResourcePath = "NineToFive.Resources.config.ini";

        static ServerConstants() {
            FileIniDataParser parser = new FileIniDataParser();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourcePath);
            using var reader = new StreamReader(stream!);
            var config = parser.ReadData(reader);

            Directory.SetCurrentDirectory(config["project"]["RootDirectory"]);
            Log.Info($"Working directory set : '{Directory.GetCurrentDirectory()}'");

            GameVersion = short.Parse(config["server"]["GameVersion"]);
            WorldCount = int.Parse(config["server"]["WorldCount"]);
            ChannelCount = int.Parse(config["server"]["ChannelCount"]);
            EnabledRanking = bool.Parse(config["server"]["EnabledRanking"]);
            EnabledSecondaryPassword = bool.Parse(config["server"]["EnabledSecondaryPassword"]);

            HostServer = config["sockets"]["HostServer"];
            CentralServer = config["sockets"]["CentralServer"];
            LoginPort = int.Parse(config["sockets"]["LoginPort"]);
            ChannelPort = int.Parse(config["sockets"]["ChannelPort"]);

            DatabaseConString = config["database"]["url"];
            Log.Info("Config loaded");
        }

        public static readonly short GameVersion;

        public static readonly string HostServer;
        public static readonly string CentralServer;

        public const short InterCentralPort = 8481;
        public const int InterChannelPort = 8482;
        public const short InterLoginPort = 8483;

        public static readonly int LoginPort;
        public static readonly int ChannelPort;

        public static readonly int WorldCount;
        public static readonly int ChannelCount;

        public static readonly bool EnabledRanking;
        public static readonly bool EnabledSecondaryPassword;

        public static readonly string DatabaseConString;

        public static readonly string[] WorldNames = {
            "Scania", "Bera", "Broa", "Windia", "Khaini", "Bellocan", "Mardia", "kradia", "Yellonde", "Demethos",
            "Galicia", "El Nido (East)", "Zenith", "Arcania", "Plana", "Kastia", "Kalluna", "Stius", "Croa", "Judis",
            "Nova", "Aster", "Cosmo", "Androa"
        };
    }

    public static class Disabled {
        public static readonly List<uint> Mobs = new List<uint>();
        public static readonly List<uint> Npcs = new List<uint>();
        public static readonly List<uint> Cash = new List<uint>();
        public static readonly List<uint> Fields = new List<uint>();
    }

    public static class JobConstants {
        public static bool IsExtendedSpJob(int jobId) => !(jobId / 1000 != 3 && jobId / 100 != 22 && jobId != 2001);
        public static bool IsEvanJob(int jobId) => jobId / 100 == 22 || jobId == 2001;

        public static int GetJobLevel(int jobId) {
            int jobStage;
            if ((jobId % 100) == 0 || jobId == 2001) return 1;
            if (jobId / 10 == 43) jobStage = (jobId - 430) / 2;
            else jobStage = jobId % 10;
            jobStage += 2;
            return jobStage >= 2 && (jobStage <= 4 || jobStage <= 10 && IsEvanJob(jobId)) ? jobStage : 0;
        }

        public static bool IsBeginnerJob(int jobId) {
            if (jobId > 6001) return jobId == 13000 || jobId == 14000;
            if (jobId >= 6000) return true;
            if (jobId > 3002) return jobId == 5000;
            if (jobId >= 3001 || jobId >= 2001 && jobId <= 2005) return true;
            return (jobId % 1000) == 0;
        }
    }

    public static class SkillConstants {
        private static bool IsIgnoreMasterLevelForCommon(int skillId) {
            if (skillId > 3220010) {
                return skillId == 32120009 || skillId == 33120010;
            }

            if (skillId >= 3220009) return true;
            if (skillId > 2120009) {
                if (skillId > 2320010) {
                    return skillId >= 3120010 && skillId <= 3120011;
                }

                return skillId == 2320010 || skillId == 2220009;
            }

            return skillId == 2120009 || skillId == 1120012 || skillId == 1220013 || skillId == 1320011;
        }

        private static int GetSkillRootFromSkill(int skillId) {
            int jobId = skillId / 10000;
            if (skillId / 10000 == 8000)
                jobId = skillId / 100;
            return jobId;
        }

        private static bool IsCommonSkill(int skillId) {
            int jobId = skillId / 10000;
            if (skillId / 10000 == 8000)
                jobId = skillId / 100;
            return jobId >= 800000 && jobId <= 800099;
        }

        private static bool IsNoviceSkill(int skillId) {
            int jobId = skillId / 10000;
            if (skillId / 10000 == 8000) {
                jobId = skillId / 100;
            }

            return JobConstants.IsBeginnerJob(jobId);
        }

        private static bool IsFieldAttackObjSkill(int skillId) {
            if (skillId == 0 || skillId < 0)
                return false;
            int jobId = skillId / 10000;
            if (skillId / 10000 == 8000)
                jobId = skillId / 100;
            return jobId == 9500;
        }

        public static bool IsSkillNeedMasterLevel(int skillId) {
            return !IsIgnoreMasterLevelForCommon(skillId)
                   && (skillId / 1000000 != 92 || (skillId % 10000) != 0)
                   && !IsCommonSkill(skillId)
                   && !IsNoviceSkill(skillId)
                   && !IsFieldAttackObjSkill(skillId)
                   && JobConstants.GetJobLevel(GetSkillRootFromSkill(skillId)) != 4;
        }
    }

    public static class ItemConstants {
        public static string GetItemCategory(int itemId) {
            if (itemId >= 2000000 && itemId <= 2500002) return "Consume";
            if (itemId >= 4000000 && itemId <= 4320000) return "Etc";
            if (itemId >= 5010000 && itemId <= 5990000) return "Cash";
            if (itemId >= 3010000 && itemId <= 3995000) return "Install";
            if (itemId >= 5000000 && itemId <= 5000107) return "Pet";
            if (itemId >= 9000000 && itemId <= 9114000) return "Special";
            if (itemId >= 1 && itemId <= 31004) return "ItemOption";
            return "";
        }

        public static InventoryType GetInventoryType(int itemId) {
            int type = itemId / 1000000;
            if (type < 1 || type > 5) {
                throw new ArgumentException($"Unknown inventory type for item {itemId}");
            }

            return (InventoryType) (type - 1);
        }

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

    public enum Jobs {
        Beginner = 0,
        Warrior = 100,
        Fighter = 110,
        Crusader = 111,
        Hero = 112,
        Page = 120,
        WhiteKnight = 121,
        Paladin = 122,
        Spearman = 130,
        DragonKnight = 131,
        DarkKnight = 132,
        Magician = 200,
        FirePoisonI = 210,
        FirePoisonII = 211,
        FirePoisonIII = 212,
        IceLightningI = 220,
        IceLightningII = 221,
        IceLightningIII = 222,
        Cleric = 230,
        Priest = 231,
        Bishop = 232,
        Archer = 300,
        Hunter = 310,
        Ranger = 311,
        BowMaster = 312,
        CrossBowman = 320,
        Sniper = 321,
        Marksman = 322,
        Rogue = 400,
        Assassin = 410,
        Hermit = 411,
        NightLord = 412,
        Bandit = 420,
        ChiefBandit = 421,
        Shadower = 422,
        BladeRecruit = 430,
        BladeAcolyte = 431,
        BladeSpecialist = 432,
        BladeLord = 433,
        DualBlade = 434,
        Brawler = 510,
        Marauder = 511,
        Buccaneer = 512,
        Gunslinger = 520,
        Outlaw = 521,
        Corsair = 522,
        CannonShooter = 501,
        Cannoneer = 530,
        CannonTrooper = 531,
        CannonMaster = 532,
        CygnusBeginner = 1000,
        DawnWarriorI = 1100,
        DawnWarriorII = 1110,
        DawnWarriorIII = 1111,
        DawnWarriorIV = 1112,
        BlazeWizardI = 1200,
        BlazeWizardII = 1210,
        BlazeWizardIII = 1211,
        BlazeWizardIV = 1212,
        WindArcherI = 1300,
        WindArcherII = 1310,
        WindArcherIII = 1311,
        WindArcherIV = 1312,
        NightWalkerI = 1400,
        NightWalkerII = 1410,
        NightWalkerIII = 1411,
        NightWalkerIV = 1412,
        ThunderBreakerI = 1500,
        ThunderBreakerII = 1510,
        ThunderBreakerIII = 1511,
        ThunderBreakerIV = 1512,
        AranBeginner = 2000,
        AranI = 2100,
        AranII = 2110,
        AranIII = 2111,
        AranIV = 2112,
        EvanBeginner = 2001,
        EvanI = 2200,
        EvanII = 2210,
        EvanIII = 2211,
        EvanIV = 2212,
        EvanV = 2213,
        EvanVI = 2214,
        EvanVII = 2215,
        EvanVIII = 2216,
        EvanIX = 2217,
        EvanX = 2218,
        MercedesBeginner = 2002,
        MercedesI = 2300,
        MercedesII = 2310,
        MercedesIII = 2311,
        MercedesIV = 2312,
        PhantomBeginner = 2003,
        PhantomI = 2400,
        PhantomII = 2410,
        PhantomIII = 2411,
        PhantomIV = 2412,
        LuminousBeginner = 2004,
        LuminousI = 2700,
        LuminousII = 2710,
        LuminousIII = 2711,
        LuminousIV = 2712,
        DemonsBeginner = 3001,
        DemonSlayerI = 3100,
        DemonSlayerII = 3110,
        DemonSlayerIII = 3111,
        DemonSlayerIV = 3112,
        DemonAvengerI = 3101,
        DemonAvengerII = 3120,
        DemonAvengerIII = 3121,
        DemonAvengerIV = 3122,
        ResistanceBeginner = 3000,
        BattleMageI = 3200,
        BattleMageII = 3210,
        BattleMageIII = 3211,
        BattleMageIV = 3212,
        WildHunterI = 3300,
        WildHunterII = 3310,
        WildHunterIII = 3311,
        WildHunterIV = 3312,
        MechanicI = 3500,
        MechanicII = 3510,
        MechanicIII = 3511,
        MechanicIV = 3512,
        XenonBeginner = 3002,
        XenonI = 3600,
        XenonII = 3610,
        XenonIII = 3611,
        XenonIV = 3612,
        MihileBeginner = 5000,
        MihileI = 5100,
        MihileII = 5110,
        MihileIII = 5111,
        MihileIV = 5112,
        KaiserBeginner = 6000,
        KaiserI = 6100,
        KaiserII = 6110,
        KaiserIII = 6111,
        KaiserIV = 6112,
        AngelicBusterBeginner = 6001,
        AngelicBusterI = 6500,
        AngelicBusterII = 6510,
        AngelicBusterIII = 6511,
        AngelicbusterIV = 6512,
        ZeroI = 10000,
        ZeroII = 10100,
        ZeroIII = 10110,
        ZeroIV = 10112,
        KinesisBeginner = 14000,
        KinesisI = 14200,
        KinesisII = 14210,
        KinesisIII = 14211,
        KinesisIV = 14212,
        PathFinderI = 301,
        PathFinderII = 330,
        PathFinderIII = 331,
        PathFinderIV = 332,
        CadenaBeginner = 6002,
        CadenaI = 6400,
        CadenaII = 6410,
        CadenaIII = 6411,
        CadenaIV = 6412,
        IlliumI = 15000,
        IlliumII = 15200,
        IlliumIII = 15211,
        IlliumIV = 15212,
        ArkBeginner = 15001,
        ArkI = 15500,
        ArkII = 15510,
        ArkIII = 15511,
        ArkIV = 15512,
        HoyoungBeginner = 16000,
        HoyoungI = 16400,
        HoyoungII = 16410,
        HoyoungIII = 16411,
        HoyoungIV = 16412,
        AdeleBeginner = 15002,
        AdeleI = 15100,
        AdeleII = 15110,
        AdeleIII = 15111,
        AdeleIV = 15112,
        GM = 900,
        Manager = 800
    }
}