using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game.Entity {
    public class Mob : Meta.Entity {
        public int ID { get; set; }
        public TemplateMob Properties { get; set; }

        public Mob(int ID)  : base(EntityType.Mob) {
            this.ID = ID;
        }
        
        public Mob(int OID, int ID, TemplateMob Template) : base(OID, EntityType.Mob) {
            this.ID = ID;
            Properties = Template;
        }
        
    }
}