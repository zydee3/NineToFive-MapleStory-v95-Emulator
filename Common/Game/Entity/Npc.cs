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
            User = new User();
            User.CharacterStat.Username = user.CharacterStat.Username;

            User.AvatarLook.Gender = user.AvatarLook.Gender;
            User.AvatarLook.Skin = user.AvatarLook.Skin;
            User.AvatarLook.Face = user.AvatarLook.Face;
            User.AvatarLook.Hair = user.AvatarLook.Hair;

            var source = user.Inventories[InventoryType.Equipped];
            var dest = User.Inventories[InventoryType.Equipped];
            foreach (var item in source.Items) {
                dest.EquipItem(new Equip(item.Id, true));
            }
        }

        public User User { get; set; }
        public Range2 HorizontalRange { get; set; }
        public string Script { get; set; }
    }
}