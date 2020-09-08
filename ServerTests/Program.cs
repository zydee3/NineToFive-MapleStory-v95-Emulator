using System;
using System.Collections.Generic;
using System.Numerics;
using MapleLib.WzLib;
using NineToFive;
using NineToFive.Game.Storage;
using NineToFive.Wz;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor
            Console.WriteLine($"Distance { test(new Vector2(2, -1), new Vector2(8, 1), new Vector2(4, 4)) }");
            
            // ReaderTest.TestSkill();
            // WzReaderTest.TestMob();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
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