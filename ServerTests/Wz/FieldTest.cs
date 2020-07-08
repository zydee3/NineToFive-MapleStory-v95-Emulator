using System.Diagnostics;
using log4net;
using NineToFive.Game;

namespace ServerTests.Wz {
    public static class FieldTest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FieldTest));

        public static void Test() {
            const int fieldId = 100000000;
            Stopwatch watch = new Stopwatch();

            watch.Start();
            Field field = new Field(fieldId);
            watch.Stop();

            foreach (var pair in field.LifePools) {
                foreach (var life in pair.Value.Values) {
                    Log.Debug($"Loaded life: {pair.Key}, {life.Id}");
                }
            }

            Log.Debug($"Loaded {field.SpawnPoints.Count} spawn points.");
            Log.Debug($"Loaded {field.Portals.Count} Portals");
            Log.Debug($"Time elapsed Loading Field({fieldId}): {watch.Elapsed}.");
        }
    }
}