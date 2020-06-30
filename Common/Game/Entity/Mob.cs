using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game.Entity {
    public class Mob : Meta.Entity {
        private TemplateMobStat Stat { get; set; }
        public Mob(int OID, TemplateMobStat Template) : base(OID, EntityType.Mob) {
            Stat = Template;
        }
        
    }
}