using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Storage;
using NineToFive.Packets;
using NineToFive.Util;

namespace NineToFive.Game.Entity {
    public class Drop : Life {
        private ItemSlot _item;
        public long ExpireTime { get; }

        public Drop(int id, int quantity, Vector2 origin, Vector2 location) : base(id, EntityType.Drop) {
            Location = location;
            Origin = origin;
            Quantity = quantity;
            ExpireTime = Time.GetFuture(180);
        }

        public Drop(ItemSlot item, Vector2 location) : base(item.TemplateId, EntityType.Drop) {
            Location = Origin = location;
            _item = item;
            ExpireTime = Time.GetFuture(180);
        }

        public Vector2 Origin { get; set; }
        public int Quantity { get; set; }

        public ItemSlot Item {
            get {
                if (_item != null) return _item;
                if (ItemConstants.GetInventoryType(TemplateId) == InventoryType.Equip) return new ItemSlotEquip(TemplateId, false, true) {Quantity = (ushort) Quantity};
                return new ItemSlotBundle(TemplateId) {Quantity = (ushort) Quantity};
            }
            
            set => _item = value;
        }
        
        public override byte[] EnterFieldPacket() {
            return DropPool.GetDropEnterField(this, 2);
        }

        public override byte[] LeaveFieldPacket() {
            return DropPool.GetDropLeaveField(this, 0);
        }
    }
}