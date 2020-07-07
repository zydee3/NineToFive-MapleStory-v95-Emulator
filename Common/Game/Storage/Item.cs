using NineToFive.Net;
using NineToFive.Util;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class Item : IPacketSerializer<Item> {
        public Item(int id, bool setItem = false) {
            Id = id;
            InventoryType = ItemConstants.GetInventoryType(id);

            if (setItem) ItemWz.SetItem(this);
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

            p.WriteInt(Id);
            if (p.WriteBool(CashItemSn > 0)) {
                // liCashItemSN->low
                // liCashItemSN->high
                p.WriteLong(CashItemSn);
            }

            p.WriteLong(DateExpire);
        }

        public virtual void Decode(Item item, Packet p) { }
    }
}