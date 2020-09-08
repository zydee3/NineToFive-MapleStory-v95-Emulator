using System;
using NineToFive.Net;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class ItemSlotBundle : ItemSlot {
        
        public string Tag { get; set; }

        public bool IsRechargable => TemplateId / 10000 == 207 || TemplateId / 10000 == 233;
        
        public ItemSlotBundle(int templateId, int quantity = 1, int slotMax = 100) : base(templateId, quantity, slotMax) {
            InventoryType = ItemConstants.GetInventoryType(templateId);
            Tag ??= "";
        }
        
        public override void Encode(Packet w) {
            base.Encode(w);
            w.WriteShort((short) Quantity);
            w.WriteString(Tag);
            w.WriteShort(Attribute);
            if (IsRechargable) {
                w.WriteLong();
            }
        }
    }
}