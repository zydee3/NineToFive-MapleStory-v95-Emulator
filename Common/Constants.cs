namespace NineToFive.Constants {
    public static class Server {
        public const int GameVersion = 95;
        public const int LoginPort = 8484;

        public static readonly string[] WorldNames = {
            "Scania", "Bera", "Broa", "Windia", "Khaini", "Bellocan", "Mardia", "kradia", "Yellonde", "Demethos",
            "Galicia", "El Nido (East)", "Zenith", "Arcania", "Plana", "Kastia", "Kalluna", "Stius", "Croa", "Judis",
            "Nova", "Aster", "Cosmo", "Androa"
        };
    }

    public static class Job {
        public static bool IsExtendedSpJob(int jobId) => !(jobId / 1000 != 3 && jobId / 100 != 22 && jobId != 2001);
    }
}