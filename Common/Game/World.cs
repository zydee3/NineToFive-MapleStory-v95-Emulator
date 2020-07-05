using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NineToFive.Constants;
using Item = NineToFive.Game.Storage.Item;


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
        public Dictionary<int, object>[] Templates;
        public Dictionary<int, Item> Items { get; set; }
        public Dictionary<int, Entity.Meta.Entity>[] Entities { get; set; }
    }
}