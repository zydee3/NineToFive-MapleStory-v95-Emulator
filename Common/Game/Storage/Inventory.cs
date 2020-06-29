using System.Collections.Generic;

namespace NineToFive.Game.Storage {
    public enum InventoryType {
        Equipped,
        Equip,
        Use,
        Setup,
        Etc,
        Cash
    }

    public class Inventory {
        private Dictionary<ushort, Item> _items = new Dictionary<ushort, Item>();

        public Inventory(InventoryType type, int size = 32) {
            Type = type;
            Size = size;
        }

        public InventoryType Type { get; }
        public int Size { get; }
        public Dictionary<ushort, Item>.ValueCollection Items => _items.Values;

        public Item this[ushort i] => _items[i];
    }
}