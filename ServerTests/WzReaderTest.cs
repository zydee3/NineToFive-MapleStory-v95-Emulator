using System.Diagnostics;
using log4net;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Wz;

namespace ServerTests {
    public static class WzReaderTest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WzReaderTest));

        public static void TestField() {
            const int fieldId = 100000000;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Field field = new Field(fieldId);
            watch.Stop();
            foreach (var pair in field.LifePools) {
                foreach (var life in pair.Value.Values) {
                    Log.Debug($"Loaded life: {pair.Key}, {life.TemplateId}");
                }
            }

            Log.Debug($"Loaded {field.SpawnPoints.Count} spawn points.");
            Log.Debug($"Loaded {field.Portals.Count} Portals");
            Log.Debug($"Time elapsed Loading Field({fieldId}): {watch.Elapsed}.");
        }

        public static void TestItem() {
            Item cash = new Item(5010008, true);
            Item consume = new Item(2000011, true);
            Item etc = new Item(04000014, true);
            Item install = new Item(03010024, true);
            Item pet = new Item(5000029, true);
        }

        public static void TestMob() {
            Mob mob = new Mob(9001005);
        }

        public static void TestSkill() {
            const int skillId = 2211006;
            const int jobId = 512;
            Stopwatch watch = new Stopwatch();
            var skill = new Skill(skillId);

            watch.Start();
            skill = new Skill(skillId);
            watch.Stop();
            Log.Debug($"Time Elapsed Loading Skill({skillId}): {watch.Elapsed}s");

            watch.Reset();

            watch.Start();
            var SS = SkillWz.GetFromJob(jobId);
            watch.Stop();
            Log.Debug($"Time Elapsed Loading Skills From Job({jobId}): {watch.Elapsed}s");

            watch.Reset();

            watch.Start();
            SS = SkillWz.GetSkills(SkillID => (SkillID > 0));
            watch.Stop();
            Log.Debug($"Time Elapsed Loading Skills Using Predicate: {watch.Elapsed}");
        }
    }
}