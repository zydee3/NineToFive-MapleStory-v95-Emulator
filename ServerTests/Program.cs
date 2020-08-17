using System;
using NineToFive;
using NineToFive.Wz;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor

            var skills = SkillWz.GetSkills();
            Console.WriteLine($"Loaded {skills.Count} skills");

            var skill = skills[13100004];
            Console.WriteLine(skill);

            // WzReaderTest.TestMob();
            // WzReaderTest.TestSkill();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
        }
    }
}