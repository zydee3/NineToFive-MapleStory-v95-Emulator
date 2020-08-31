using System;
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

        public int GetHoldableQuantity(Item item) {
            if (Items.Count >= Size) return 0;
            int remaining = item.Quantity;
            byte slot = 0;
            
            while (remaining > 0 && ++slot <= Size) {
                Item current = this[slot];
                if (current == null) {
                    remaining -= item.SlotMax;
                } else if (current.Id == item.Id) {
                    remaining -= (item.SlotMax - current.Quantity);
                }
            }
            
            return item.Quantity - Math.Max(0, remaining);
        }

        public void AddItem(Item item) {
            if (Items.Count >= Size) return;
            ushort slotMax = (ushort) item.SlotMax;
            
            for (byte slot = 1; slot <= Size; slot++) {
                if (this[slot] == null) { // add to empty slot
                    if(item.Quantity > slotMax) { // deposit to slot max quantity of item in slot and continue with remaining
                        item.Quantity -= slotMax;
                        Item fullStackItem = new Item(item.Id) { Quantity = slotMax };
                        this[slot] = fullStackItem;
                    } else { // slot can hold the max quantity, put everything in and we're done
                        this[slot] = item;
                        return;
                    }
                }

                Item current = this[slot];
                if(current.Id == item.Id){ // another instance of item found, merge the two instances
                    if (item.Quantity + current.Quantity > slotMax) { // two instances total cannot fit in slot, max current slot and continue
                        item.Quantity = (ushort) (slotMax - current.Quantity);
                        current.Quantity = slotMax;
                    } else { // two instances total do fit in slot, so combine them and delete previous item
                        current.Quantity = (ushort) (slotMax + current.Quantity);
                        item = null;
                        return;
                    }
                }
            }
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