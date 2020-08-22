using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IniParser;
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
            if (skillId <= 2120009) return skillId == 2120009 || skillId == 1120012 || skillId == 1220013 || skillId == 1320011;
            if (skillId > 2320010) {
                return skillId >= 3120010 && skillId <= 3120011;
            }

            return skillId == 2320010 || skillId == 2220009;

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
            if (IsIgnoreMasterLevelForCommon(skillId)) return false;
            var jobId = skillId / 10000;
            var jobLevel = JobConstants.GetJobLevel(jobId);
            if (jobId == 2001) {
                return jobLevel == 9 || jobLevel == 10 || skillId == 22111001 || skillId == 22141002 || skillId == 22140000;
            }

            if (jobId / 10 == 43) {
                return jobLevel == 4;
            }

            return jobId % 10 == 2;
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

        public static string GetEquipCategory(int equipId) {
            switch (Math.Floor(equipId / 10000.0)) {
                case 101: 
                case 102:
                case 103:
                case 112:
                case 113:
                case 114:
                case 115:
                    return "Accessory";
                case 100:
                    return "Cap";
                case 110:
                    return "Cape";
                case 104:
                    return "Coat";
                case 194:
                case 195:
                case 196:
                case 197:
                    return "Dragon";
                case 108:
                    return "Glove";
                case 105:
                    return "Longcoat";
                case 161:
                case 162:
                case 163:
                case 164:
                case 165:
                    return "Mechanic";
                case 106:
                    return "Pants";
                case 181:
                case 182:
                case 183:
                    return "PetEquip";
                case 111:
                    return "Ring";
                case 109:
                    return "Shield";
                case 107:
                    return "Shoes";
                case 190:
                case 191:
                case 193:
                case 198:
                    return "TamingMob";
                default:
                    if (equipId >= 1302000 && equipId <= 1702301) return "Weapon";
                    if (equipId >= 20000 && equipId <= 22000) return "Face";
                    if (equipId >= 30000 && equipId <= 35000) return "Hair";
                    return "";
            }
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
}