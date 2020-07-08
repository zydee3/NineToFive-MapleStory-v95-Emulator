using System;

namespace NineToFive.Util {
    public static class Randomizer {
        private static readonly Random Rand = new Random();

        public static int GetInt() {
            return Rand.Next();
        }

        public static int GetInt(int toExclusive) {
            return Rand.Next(toExclusive);
        }

        public static int GetInt(int fromInclusive, int toExclusive) {
            return Rand.Next(fromInclusive, toExclusive);
        }

        public static void GetBytes(byte[] buffer) {
            Rand.NextBytes(buffer);
        }

        public static double GetDouble() {
            return Rand.NextDouble();
        }

        public static uint GetUInt() {
            byte[] buf = new byte[4];
            Rand.NextBytes(buf);
            return BitConverter.ToUInt32(buf, 0);
        }
    }
}