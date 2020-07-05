using System;
using System.Collections.Generic;
using System.Diagnostics;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace ServerTests.Wz {
    public class FieldTest {
        public static void Test() {
            const int fieldId = 610020005;
            Stopwatch Watch = new Stopwatch();

            Watch.Start();
            Field Field = new Field(fieldId,  1);
            Watch.Stop();

            foreach ((EntityType Type, var Entities) in Field.Life) {
                foreach ((int ID, Entity Entity) in Entities) {
                    Console.WriteLine($"Loaded Entity: ID={ID}, Type={Type}.");
                }
            }

            Console.WriteLine($"Loaded {Field.SpawnPoints.Count} spawn points.");
            Console.WriteLine($"Loaded {Field.Portals.Length} Portals");
            Console.WriteLine($"Time Elpased Loading Field({fieldId}): {Watch.Elapsed}.");
        }
    }
}