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
        private readonly Dictionary<short, ItemSlot> _items = new Dictionary<short, ItemSlot>();

        public Inventory(InventoryType type, byte size = 32) {
            Type = type;
            Size = (type == InventoryType.Equipped) ? byte.MaxValue : size;
        }

        public InventoryType Type { get; }
        public byte Size { get; }
        public Dictionary<short, ItemSlot>.ValueCollection Items => _items.Values;

        public bool EquipItem(ItemSlotEquip equip, bool replace = false) {
            equip.BagIndex = (short) -ItemConstants.GetBodyPartFromId(equip.TemplateId);
            if (replace) _items.Remove(equip.BagIndex);
            return _items.TryAdd(equip.BagIndex, equip);
        }

        public List<InventoryUpdateEntry> EquipItem(Inventory equippedInventory, sbyte from, sbyte to) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();

            if (Type == InventoryType.Equip && equippedInventory.Type == InventoryType.Equipped) {
                ItemSlot equipToAdd, equipInTheWay = equippedInventory.Remove(to);
                
                if (equipInTheWay != null 
                    && (equipToAdd = Remove(from)) != null
                    && equippedInventory.Insert(equipToAdd, to)
                    && Insert(equipInTheWay, from))
                {
                    updates.Add(new InventoryUpdateEntry(ref equipToAdd, InventoryOperation.Move, from));
                    updates.Add(new InventoryUpdateEntry(ref equipInTheWay, InventoryOperation.Move, to));
                } else if ((equipToAdd = Remove(from)) != null
                    && equippedInventory.Insert(equipToAdd, to))
                {
                    updates.Add(new InventoryUpdateEntry(ref equipToAdd, InventoryOperation.Move, from));
                }
            }
            
            return updates;
        }

        public List<InventoryUpdateEntry> UnequipItem(Inventory equipInventory, sbyte from, sbyte to) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            ItemSlot equip;

            if (Type == InventoryType.Equipped               // are we taking from equipped?
            && equipInventory.Type == InventoryType.Equip    // is the receiving inventory equip?
            && equipInventory[to] == null                    // is the receiving slot empty?
            && (equip = Remove(from)) != null                // does the item we're un-equipping exist?
            && equipInventory.Insert(equip, to))             // was the equip successfully added? this last bool should never fail honestly
                updates.Add(new InventoryUpdateEntry(ref equip, InventoryOperation.Move, from));
            
            return updates;
        }

        /// <summary>
        /// Checks how much of the item can be held. This is used because GMS picks up as much as you can hold, leaving the remainder on the floor.
        /// </summary>
        /// <param name="item">Item to be picked up.</param>
        /// <returns>Quantity of item that can be held.</returns>
        public int GetHoldableQuantity(ItemSlot item) {
            if (Items.Count >= Size) return 0;
            int remaining = item.Quantity;
            sbyte slot = 0;
            
            while (remaining > 0 && ++slot <= Size) {
                ItemSlot current = this[slot];
                if (current == null) {
                    remaining -= item.SlotMax;
                } else if (current.TemplateId == item.TemplateId) {
                    remaining -= (item.SlotMax - current.Quantity);
                }
            }
            
            return item.Quantity - Math.Max(0, remaining);
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        public List<InventoryUpdateEntry> AddItem(ItemSlot item) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            
            if (Items.Count >= Size) return updates;
            ushort slotMax = (ushort) item.SlotMax;
            
            for (sbyte slot = 1; slot <= Size; slot++) {
                if (this[slot] == null) {
                    if (item.Quantity <= slotMax && Insert(item, slot)) {
                        updates.Add(new InventoryUpdateEntry(ref item, InventoryOperation.Add));
                        break;
                    }

                    ItemSlot newSlotItem = null;
                    switch (item) {
                        case ItemSlotBundle bundle:
                            newSlotItem = new ItemSlotBundle(bundle.TemplateId, slotMax, slotMax);
                            break;
                        case ItemSlotEquip equip:
                            newSlotItem = new ItemSlotEquip(equip.TemplateId);
                            break;
                        case ItemSlotPet pet:
                            newSlotItem = new ItemSlotPet(item.TemplateId);
                            break;
                    }

                    if (newSlotItem == null) return updates;
                    if (Insert(newSlotItem, slot)) {
                        item.Quantity -= slotMax;
                        updates.Add(new InventoryUpdateEntry(ref newSlotItem, InventoryOperation.Add));
                    }
                }

                ItemSlot current = this[slot];
                if(Type != InventoryType.Equip && Type != InventoryType.Equipped && item.TemplateId == current.TemplateId) {
                    int remaining = Merge(item, current);
                    updates.Add(new InventoryUpdateEntry(ref current, InventoryOperation.Update));
                    if (remaining == 0) break;
                }
            }
            
            return updates;
        }

        public List<InventoryUpdateEntry> MoveItem(sbyte from, sbyte to) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            ItemSlot itemToMove = this[from], itemInTheWay = this[to];

            if (itemToMove != null) {
                if (itemInTheWay == null) {
                    itemToMove.BagIndex = to;
                    _items.Remove(from);
                    _items.TryAdd(to, itemToMove);
                    updates.Add(new InventoryUpdateEntry(ref itemToMove, InventoryOperation.Move, from));
                } else {
                    if (itemToMove.TemplateId == itemInTheWay.TemplateId) {
                        if (itemInTheWay.Quantity < itemInTheWay.SlotMax) {
                            int remaining = Merge(itemToMove, itemInTheWay);
                            updates.Add(new InventoryUpdateEntry(ref itemToMove, remaining == 0 ? InventoryOperation.Remove : InventoryOperation.Update));
                            updates.Add(new InventoryUpdateEntry(ref itemInTheWay, InventoryOperation.Update));
                        }
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

        public List<InventoryUpdateEntry> UseItem(sbyte slot, int quantity) {
            List<InventoryUpdateEntry> updates = new List<InventoryUpdateEntry>();
            ItemSlot item = this[slot];
            if (!(item is ItemSlotBundle bundle)) return updates;

            bool usable = bundle.Quantity > 0;
            if (!usable || (bundle.Quantity -= (ushort) quantity) == 0) {
                Remove(slot, false);
                updates.Add(new InventoryUpdateEntry(ref item, InventoryOperation.Remove) { Complete = usable });
                return updates;
            }

            updates.Add(new InventoryUpdateEntry(ref item, InventoryOperation.Update) );
            return updates;
        }

        public bool Insert(ItemSlot item, sbyte slot) {
            if (item == null || this[slot] != null) return false;
            item.BagIndex = slot;
            return _items.TryAdd(slot, item);
        }
        
        private ushort Merge(ItemSlot origin, ItemSlot target) {
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

        private bool Swap(ItemSlot first, ItemSlot second) {
            if (first == null || second == null) return false;
            
            _items.Remove(first.BagIndex);
            _items.Remove(second.BagIndex);

            short temp = first.BagIndex;
            first.BagIndex = second.BagIndex;
            second.BagIndex = temp;

            return _items.TryAdd(first.BagIndex, first) 
                   && _items.TryAdd(second.BagIndex, second);
        }

        public ItemSlot Remove(sbyte slot, bool resetSlot = true) {
            ItemSlot target = this[slot];
            if (target != null) {
                if(resetSlot) target.BagIndex = -1;
                _items.Remove(slot);
            }

            return target;
        }
        
        public ItemSlot this[short bagIndex] {
            get {
                _items.TryGetValue(bagIndex, out ItemSlot item);
                return item;
            }
            set => _items.Add(bagIndex, value);
        }
    }
}