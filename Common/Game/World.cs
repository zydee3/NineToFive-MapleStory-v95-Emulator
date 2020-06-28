using System.Collections.Concurrent;
using NineToFive.Constants;

namespace NineToFive.Game {
    public class World {
        public World(byte id) {
            Id = id;
        }

        public static byte ActiveWorld { get; set; }

        public byte Id { get; }
        public string Name => ServerConstants.WorldNames[Id];
        public Channel[] Channels { get; internal set; }
        public ConcurrentDictionary<uint, User> Users { get; } = new ConcurrentDictionary<uint, User>();
    }
}