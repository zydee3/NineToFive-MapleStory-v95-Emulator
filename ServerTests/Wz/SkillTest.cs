using System;
using System.Diagnostics;
using NineToFive.Game;
using NineToFive.Wz;

namespace ServerTests.Wz {
    public static class SkillTest {
        private const int SkillID = 2211006;
        private const int JobID = 512;
        
        public static void Test() {
            Stopwatch watch = new Stopwatch();

            
            var fake = new Skill(SkillID);
            
            watch.Start();
            var S = new Skill(SkillID);
            watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skill({SkillID}): {watch.Elapsed}s");
            
            watch.Reset();
            
            watch.Start();
            var SS = SkillWz.GetFromJob(JobID);
            watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skills From Job({JobID}): {watch.Elapsed}s");
            
            watch.Reset();
            
            watch.Start();
            var SSS = SkillWz.GetSkills(SkillID => (SkillID > 0));
          
            watch.Stop();
            Console.WriteLine($"Time Elapsed Loading Skills Using Predicate: {watch.Elapsed}");
            
        }
    }
}