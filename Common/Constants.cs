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
        public const int ChannelCount = 1;

        public static readonly string[] WorldNames = {
            "Scania", "Bera", "Broa", "Windia", "Khaini", "Bellocan", "Mardia", "kradia", "Yellonde", "Demethos",
            "Galicia", "El Nido (East)", "Zenith", "Arcania", "Plana", "Kastia", "Kalluna", "Stius", "Croa", "Judis",
            "Nova", "Aster", "Cosmo", "Androa"
        };
    }

    public static class Job {
        public static bool IsExtendedSpJob(int jobId) => !(jobId / 1000 != 3 && jobId / 100 != 22 && jobId != 2001);
    }
    
    public enum SkillProperties {
        acc, asrR, attackCount, bulletCount, cooltime, 
        cr, criticaldamageMax, criticaldamageMin, damage, 
        damR, dot, dotInterval, dotTime, emdd, epad, epdd, 
        er, eva, expR, hpCon, ignoreMobpdpR, jump, mad, 
        mastery, maxLevel, mdd, mhpR, mobCount, morph, mp, 
        mpCon, pad, padX, pdd, pddR, prop, range, speed, 
        subProp, subTime, t, terR, time, u, v, w, x, y, z
    }
}