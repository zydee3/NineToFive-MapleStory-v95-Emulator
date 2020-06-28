using System.Collections.Concurrent;

namespace NineToFive.Game {
    public class World {
        public byte Id { get; }
        public Channel[] Channels { get; internal set; }
        public string Name => Constants.Server.WorldNames[Id];
        public ConcurrentDictionary<uint,User> Users { get; } = new ConcurrentDictionary<uint, User>();

        public World(byte id) {
            Id = id;
        }
    }
}