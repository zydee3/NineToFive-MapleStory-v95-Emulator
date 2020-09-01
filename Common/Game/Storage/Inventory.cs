using System;
using System.Collections.Generic;
using System.Linq;
using NineToFive.Constants;
using NineToFive.Game.Storage.Meta;

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

        /// <summary>
        /// Checks how much of the item can be held. This is used because GMS picks up as much as you can hold, leaving the remainder on the floor.
        /// </summary>
        /// <param name="item">Item to be picked up.</param>
        /// <returns>Quantity of item that can be held.</returns>
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

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        public List<InventoryUpdateEntry> AddItem(Item item) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            
            if (Items.Count >= Size) return updates;
            ushort slotMax = (ushort) item.SlotMax;
            
            for (byte slot = 1; slot <= Size; slot++) {
                if (this[slot] == null) {
                    if (item.Quantity <= slotMax && Insert(item, slot)) {
                        updates.Add(new InventoryUpdateEntry(ref item, InventoryOperation.Add));
                        break;
                    }
                    
                    Item newSlotItem = new Item(item.Id) {Quantity = slotMax};
                    if (Insert(newSlotItem, slot)) {
                        item.Quantity -= slotMax;
                        updates.Add(new InventoryUpdateEntry(ref newSlotItem, InventoryOperation.Add));
                    }
                }

                Item current = this[slot];
                if(item.Id == current.Id) {
                    int remaining = Merge(item, current);
                    updates.Add(new InventoryUpdateEntry(ref current, InventoryOperation.Update));
                    if (remaining == 0) break;
                }
            }
            
            return updates;
        }

        public List<InventoryUpdateEntry> MoveItem(byte from, byte to) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            Item itemToMove = this[from], itemInTheWay = this[to];

            if (itemToMove != null) {
                if (itemInTheWay == null) {
                    itemToMove.BagIndex = to;
                    _items.Remove(from);
                    _items.TryAdd(to, itemToMove);
                    updates.Add(new InventoryUpdateEntry(ref itemToMove, InventoryOperation.Move, from));
                } else {
                    if (itemToMove.Id == itemInTheWay.Id) {
                        int remaining = Merge(itemToMove, itemInTheWay);
                        updates.Add(new InventoryUpdateEntry(ref itemToMove, remaining == 0 ? InventoryOperation.Remove : InventoryOperation.Update));
                        updates.Add(new InventoryUpdateEntry(ref itemInTheWay, InventoryOperation.Update));
                    } else {
                        if (Swap(itemToMove, itemInTheWay)) {
                            updates.Add(new InventoryUpdateEntry(ref itemToMove, InventoryOperation.Move, from));
                            updates.Add(new InventoryUpdateEntry(ref itemInTheWay, InventoryOperation.Update, to));
                        }
                    }
                }
            }

            return updates;
        }

        private bool Insert(Item item, byte slot) {
            if (item == null || this[slot] != null) return false;
            item.BagIndex = slot;
            return _items.TryAdd(slot, item);
        }
        
        private ushort Merge(Item origin, Item target) {
            if (origin == null || target == null) return 0;
            if (_items.ContainsKey(origin.BagIndex)) _items.Remove(origin.BagIndex);

            int slotMax = target.SlotMax;
            int originQuantity = origin.Quantity;
            int targetQuantity = target.Quantity;

            if (originQuantity + targetQuantity > slotMax) {
                target.Quantity = (ushort) slotMax;
                origin.Quantity -= (ushort) (slotMax - targetQuantity);
            } else {
                target.Quantity += (ushort) originQuantity;
                origin.Quantity = 0;
            }
            
            return origin.Quantity;
        }

        private bool Swap(Item first, Item second) {
            if (first == null || second == null) return false;
            
            _items.Remove(first.BagIndex);
            _items.Remove(second.BagIndex);

            short temp = first.BagIndex;
            first.BagIndex = second.BagIndex;
            second.BagIndex = temp;

            return _items.TryAdd(first.BagIndex, first) 
                   && _items.TryAdd(second.BagIndex, second);
        }

        public Item Remove(byte slot) {
            Item target = this[slot];
            if (target != null) {
                target.BagIndex = -1;
                _items.Remove(slot);
            }

            return target;
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