using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Game.Storage {
    public abstract class ItemSlot : IPacketSerializer<ItemSlot> {
        
        public int TemplateId { get; }
        public long CashItemSN { get; set; } // FILETIME
        public long DateExpire { get; set; }
        public short Attribute { get; set; }

        public short BagIndex { get; set; }
        public uint GeneratedId { get; set; }
        
        public ushort Quantity { get; set; }
        public int SlotMax { get; }
        public virtual byte Type => 2;
        public InventoryType InventoryType { get; set; }
        
        protected ItemSlot(int templateId, int quantity = 1, int slotMax = 1) {
            TemplateId = templateId;
            Quantity = (ushort) quantity;
            SlotMax = slotMax;
        }
        
        public virtual void Encode(ItemSlot itemSlot, Packet p) {
            p.WriteByte(itemSlot.Type);
            p.WriteInt(itemSlot.TemplateId);
            if (p.WriteBool(itemSlot.CashItemSN > 0)) {
                // liCashItemSN->low
                // liCashItemSN->high
                p.WriteLong(itemSlot.CashItemSN);
            }

            // p.WriteLong(DateTime.FromFileTimeUtc(150842304000000000).ToFileTime()); // No expiration
            p.WriteLong(itemSlot.DateExpire);
        }

        public virtual void Decode(ItemSlot itemSlot, Packet p) { }
    }
}