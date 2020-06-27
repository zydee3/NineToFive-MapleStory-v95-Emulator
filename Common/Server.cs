using System;
using NineToFive.Game;

namespace NineToFive {
    public class Server {
        public static World[] Worlds { get; private set; }

        public static void Initialize() {
            Worlds = new World[1];
            for (byte a = 0; a < Worlds.Length; a++) {
                World world = new World(a);
                world.Channels = new Channel[1];
                for (byte b = 0; b < world.Channels.Length; b++) {
                    world.Channels[b] = new Channel(b);
                }

                Worlds[a] = world;

                Console.WriteLine($"Instantiated world {(a + 1)} - {world.Channels.Length} channels");
            }
        }
    }
}