using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Game.Storage {
    public class Item : IPacketSerializer<Item> {
        public Item(int id) {
            Id = id;
        }

        public int Id { get; }
        public sbyte BagIndex { get; set; }

        public virtual void Encode(Item item, Packet p) { }

        public virtual void Decode(Item item, Packet p) { }
    }
}