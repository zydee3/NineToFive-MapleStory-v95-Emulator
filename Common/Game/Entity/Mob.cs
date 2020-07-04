using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Wz;

namespace NineToFive.Game.Entity {
    public class Mob : Meta.Entity {
        public TemplateMob Properties { get; set; }
        public int HP { get; set; }

        public Mob(int ID) : base(EntityType.Mob) {
            MobWz.SetMob(this, ID);
        }
    }
}