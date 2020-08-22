using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Packets;

namespace NineToFive.Game.Entity {
    public class Npc : Life {
        public Npc(int templateId) : base(templateId, EntityType.Npc) {
            TemplateId = templateId;
        }

        public override byte[] EnterFieldPacket() {
            return NpcPool.GetNpcEnterField(this);
        }

        public override byte[] LeaveFieldPacket() {
            return NpcPool.GetNpcLeaveField(this);
        }

        public void ImitateUser(User user) {
            AvatarLook = new AvatarLook() {
                Gender = user.AvatarLook.Gender,
                Skin = user.AvatarLook.Skin,
                Face = user.AvatarLook.Face,
                Hair = user.AvatarLook.Hair
            };
            Equipped = new Inventory(InventoryType.Equipped);
            foreach (var item in user.Inventories[InventoryType.Equipped].Items) {
                Equipped.EquipItem(new Equip(item.Id, true));
            }
        }

        public Inventory Equipped { get; set; }
        public AvatarLook AvatarLook { get; set; }
        public Range2 HorizontalRange { get; set; }
        public string Script { get; set; }
    }
}