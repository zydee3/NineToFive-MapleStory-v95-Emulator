using System;
using System.Diagnostics;
using System.Numerics;
using NineToFive;
using NineToFive.Constants;
using NineToFive.Resources;
using NineToFive.Wz;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor

            Stopwatch watch = new Stopwatch();
            watch.Start();
            int count = SkillWz.LoadSkills();
            watch.Stop();
            Console.WriteLine($"Loaded {count} skills in {watch.ElapsedMilliseconds}ms");

            var skill = WzCache.Skills[(int) Skills.AssassinHaste];
            byte[] flags = new byte[16];

            foreach (var pair in skill.CTS) {
                var ts = pair.Key;
                int byteIndex = (int) ts / 8;
                int bitIndex = (int) ts;
                flags[byteIndex] |= (byte) (bitIndex % 0x80);
                Console.WriteLine($"{nameof(byteIndex)}={byteIndex}, {nameof(bitIndex)}={bitIndex}");
            }

            foreach (var flag in flags) {
                Console.WriteLine(Convert.ToString(flag), 2);
            }

            // Console.WriteLine($"Distance { test(new Vector2(2, -1), new Vector2(8, 1), new Vector2(4, 4)) }");
        }

        public static double test(Vector2 point1, Vector2 point2, Vector2 original) {
            if (point2.X - point1.X == 0) return int.MaxValue;

            double m = (point2.Y - point1.Y) / (point2.X - point1.X);
            double b = point2.Y - m * point2.X;

            double y = m * original.X + b; // (original.X, y)
            return original.Y - y;
        }
    }
}