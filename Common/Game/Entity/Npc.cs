using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Npc : Life {
        public Npc(int templateId) : base(templateId, EntityType.Npc) { }

        public override byte[] EnterFieldPacket() {
            return NpcPool.GetNpcEnterField(this);
        }

        public override byte[] LeaveFieldPacket() {
            return NpcPool.GetNpcLeaveField(this);
        }
        
        public AvatarLook AvatarLook { get; set; }
        public Range2 HorizontalRange { get; set; }
    }
}