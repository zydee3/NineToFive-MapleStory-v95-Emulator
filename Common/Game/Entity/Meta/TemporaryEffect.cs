using System.Threading.Tasks;
using NineToFive.Game.Storage;

namespace NineToFive.Game.Entity.Meta {
    public class TemporaryEffect {

        private readonly bool _isActive = true;
        private uint buffFlag = 0;
        private uint statFlag = 0;
        
        public TemporaryEffect(ref ItemSlot item) {
            
        }

        public TemporaryEffect(Skill skill, int level) {
            _isActive = skill.Time[level] >= 1000;
        }

        public void Apply(User user) {
            
        }
    }
}