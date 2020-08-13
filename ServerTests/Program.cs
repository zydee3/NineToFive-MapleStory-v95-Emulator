using System;
using NineToFive;
using NineToFive.Game.Entity;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            // short gver = ServerConstants.GameVersion; // proc static constructor
            // WzReaderTest.TestMob();
            // WzReaderTest.TestSkill();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
            
            WeakReference<User> controller = new WeakReference<User>(null);
            bool result = controller.TryGetTarget(out var user);
            Console.WriteLine("RESULT : "  + result);
            Console.WriteLine("VALUE : "  + (user == null ? "null" : "nah"));
        }
    }
}