using System;
using System.Diagnostics;
using NineToFive.Game;

namespace ServerTests.Wz {
    public class FieldTest {
        public static void Test() {
            const uint FieldID = 211041000;
            Stopwatch Watch = new Stopwatch();

            Watch.Start();
            Field Field = new Field( FieldID,  1);
            Watch.Stop();

            Console.WriteLine($"Time Elpased Loading Field({FieldID}): {Watch.Elapsed}");
        }
    }
}