using System;
using NineToFive.Net;
using NineToFive.Util;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class Item : IPacketSerializer<Item> {
        public Item(int id, bool setItem = false) {
            Id = id;
            InventoryType = ItemConstants.GetInventoryType(id);

            if (setItem) ItemWz.SetItem(this);
            
            //todo need to load item from database
        }

        public override string ToString() {
            return $"Item{{ID: {Id}, BagIndex: {BagIndex}}}";
        }

        public virtual byte Type => 2;
        public InventoryType InventoryType { get; }
        public int Id { get; }
        public uint GeneratedId { get; set; }
        public short BagIndex { get; set; }
        public virtual ushort Quantity { get; set; }
        public long CashItemSn { get; set; } // FILETIME
        public long DateExpire { get; set; }

        public virtual void Encode(Item item, Packet p) {
            p.WriteByte(item.Type);
            p.WriteInt(item.Id);
            if (p.WriteBool(item.CashItemSn > 0)) {
                // liCashItemSN->low
                // liCashItemSN->high
                p.WriteLong(item.CashItemSn);
            }

            // p.WriteLong(DateTime.FromFileTimeUtc(150842304000000000).ToFileTime()); // No expiration
            p.WriteLong(item.DateExpire);
        }

        public virtual void Decode(Item item, Packet p) { }
    }
}