using System;
using System.Diagnostics;
using NineToFive.Game;

namespace ServerTests.Wz {
    public class FieldTest {
        public static void Test() {
            const int fieldId = 100000000;
            Stopwatch watch = new Stopwatch();

            watch.Start();
            Field field = new Field(fieldId, 1);
            watch.Stop();

            foreach (var pair in field.LifePools) {
                foreach (var life in pair.Value.Values) {
                    Console.WriteLine($"Loaded life: {pair.Key}, {life.Id}");
                }
            }

            Console.WriteLine($"Loaded {field.SpawnPoints.Count} spawn points.");
            Console.WriteLine($"Loaded {field.Portals.Length} Portals");
            Console.WriteLine($"Time elapsed Loading Field({fieldId}): {watch.Elapsed}.");
        }
    }
}