using System;
using System.Diagnostics;
using NineToFive.Game;
using NineToFive.Wz;

namespace ServerTests.Wz {
    public class SkillTest {
        private const int SkillID = 2211006;
        private const int JobID = 512;
        
        public static void Test() {
            Stopwatch Watch = new Stopwatch();

            
            var fake = new Skill(SkillID);
            
            Watch.Start();
            var S = new Skill(SkillID);
            Watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skill({SkillID}): {Watch.Elapsed}s");
            
            Watch.Reset();
            
            Watch.Start();
            var SS = SkillWz.GetFromJob(JobID);
            Watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skills From Job({JobID}): {Watch.Elapsed}s");
            
            Watch.Reset();
            
            Watch.Start();
            var SSS = SkillWz.GetSkills(SkillID => (SkillID > 0));
          
            Watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skills Using Predicate: {Watch.Elapsed}");
            
        }
    }
}