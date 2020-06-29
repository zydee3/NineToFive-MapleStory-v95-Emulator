using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game.Entity {
    public class Mob {
        private TemplateMobStat Stat { get; set; }
        public Mob(TemplateMobStat Template) {
            Stat = Template;
        }
        
    }
}