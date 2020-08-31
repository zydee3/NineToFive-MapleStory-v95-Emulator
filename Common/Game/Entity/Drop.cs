using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Storage;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Drop : Life {
        public Drop(int id, Life creator, bool money = false) : base(money ? 0 : id, EntityType.Drop) {
            Fh = creator.Fh;
            Location = creator.Location;
            Origin = creator.Location;

            if (money) Money = id;
        }

        public override byte[] EnterFieldPacket() {
            return DropPool.GetDropEnterField(this, 2);
        }

        public override byte[] LeaveFieldPacket() {
            return DropPool.GetDropLeaveField(this, 0);
        }

        public Vector2 Origin { get; set; }
        public int Money { get; set; }
        
        public Item Item => new Item((int) Id);
    }
}