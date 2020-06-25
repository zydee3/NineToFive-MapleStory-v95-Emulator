using System;

namespace NineToFive.Util {
    class RNG {

        private static readonly Random rand = new Random();

        public static uint GetUInt() {
            byte[] buf = new byte[4];
            rand.NextBytes(buf);
            return BitConverter.ToUInt32(buf, 0);
        }
    }
}
