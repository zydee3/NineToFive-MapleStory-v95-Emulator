using NineToFive.Constants;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Reactor : Life {
        public Reactor(int templateId) : base(templateId, EntityType.Reactor) { }

        public override byte[] EnterFieldPacket() {
            return ReactorPool.GetReactorEnterField(this);
        }

        public override byte[] LeaveFieldPacket() {
            return ReactorPool.GetReactorLeaveField(this);
        }
    }
}