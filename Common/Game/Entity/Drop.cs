using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Storage;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Drop : Life {
        private ItemSlot _item;
        public long ExpireTime { get; set; }
        
        public Drop(int id, int quantity, Life creator) : base(id, EntityType.Drop) {
            Fh = creator.Fh;
            Location = creator.Location;
            Origin = creator.Location;
            Quantity = quantity;
        }

        public Drop(ItemSlot item, Life creator) : base(item.TemplateId, EntityType.Drop) {
            Fh = creator.Fh;
            Location = creator.Location;
            Origin = creator.Location;
            _item = item;
        }

        public Drop(int id, int quantity, Vector2 origin, Vector2 location) : base(id, EntityType.Drop) {
            Location = location;
            Origin = origin;
            Quantity = quantity;
        }

        public Drop(ItemSlot item, Vector2 location) : base(item.TemplateId, EntityType.Drop) {
            Location = Origin = location;
            _item = item;
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