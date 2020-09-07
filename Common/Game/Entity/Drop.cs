using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Storage;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Drop : Life {
        private Item _item;
        
        
        public Drop(int id, int quantity, Life creator) : base(id, EntityType.Drop) {
            Fh = creator.Fh;
            Location = creator.Location;
            Origin = creator.Location;
            Quantity = quantity;
        }

        public Drop(Item item, Life creator) : base(item.Id, EntityType.Drop) {
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

        public Drop(Item item, Vector2 location) : base(item.Id, EntityType.Drop) {
            Location = Origin = location;
            _item = item;
        }

        public Vector2 Origin { get; set; }
        public int Quantity { get; set; }

        public Item Item {
            get {
                if (_item != null) return _item;
                if (ItemConstants.GetInventoryType(TemplateId) == InventoryType.Equip) return new Equip(TemplateId, false, true) {Quantity = (ushort) Quantity};
                return new Item(TemplateId, true) {Quantity = (ushort) Quantity};
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