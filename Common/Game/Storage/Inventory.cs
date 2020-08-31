using System.Collections.Generic;
using System.Linq;

namespace NineToFive.Game.Storage {
    public enum InventoryType {
        Equip,
        Use,
        Setup,
        Etc,
        Cash,
        Equipped,
    }

    public class Inventory {
        private readonly Dictionary<short, Item> _items = new Dictionary<short, Item>();

        public Inventory(InventoryType type, byte size = 32) {
            Type = type;
            Size = (type == InventoryType.Equipped) ? byte.MaxValue : size;
        }

        public InventoryType Type { get; }
        public byte Size { get; }
        public Dictionary<short, Item>.ValueCollection Items => _items.Values;

        public bool EquipItem(Equip equip, bool replace = false) {
            equip.BagIndex = (short) -ItemConstants.GetBodyPartFromId(equip.Id);
            if (replace) _items.Remove(equip.BagIndex);
            return _items.TryAdd(equip.BagIndex, equip);
        }

        public bool AddItem(Item item) {
            if (Items.Count >= Size) return false;
            Item found = _items.Values.FirstOrDefault(inventoryItem => inventoryItem.Id == item.Id);
            if (found != null) {
                
            }
            return true;
        }

        public Item this[short bagIndex] {
            get {
                _items.TryGetValue(bagIndex, out Item item);
                return item;
            }
            set => _items.Add(bagIndex, value);
        }
    }
}