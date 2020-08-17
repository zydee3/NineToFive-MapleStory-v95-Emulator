using System;
using NineToFive;
using NineToFive.Game.Entity;
using NineToFive.Wz;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor

            SkillWz.GetSkills();

            // WzReaderTest.TestMob();
            // WzReaderTest.TestSkill();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
        }
    }
}