using System.Numerics;
using NineToFive.Constants;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Drop : Life {
        public Drop(int id, Life creator) : base(id, EntityType.Drop) {
            Fh = creator.Fh;
            Location = creator.Location;
            Origin = creator.Location;
        }

        public override byte[] EnterFieldPacket() {
            return DropPool.GetDropEnterField(this, 1);
        }

        public override byte[] LeaveFieldPacket() {
            return DropPool.GetDropLeaveField(this, 0);
        }

        public Vector2 Origin { get; set; }
    }
}