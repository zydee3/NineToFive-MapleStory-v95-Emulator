using System;
using NineToFive;
using NineToFive.Wz;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor

            var skills = SkillWz.LoadSkills();
            Console.WriteLine($"Loaded {skills.Count} skills");
            Console.WriteLine("=============================");

            var skill = skills[9101003];
            Console.WriteLine(skill);

            // WzReaderTest.TestMob();
            // WzReaderTest.TestSkill();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
        }
    }
}