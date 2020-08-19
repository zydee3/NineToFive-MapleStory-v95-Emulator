using NineToFive.Wz;

namespace NineToFive.Game.Entity.Meta {
    public class SkillRecord {
        public SkillRecord(int id, int level) {
            Id = id;
            Level = level;
        }

        public int Id { get; }
        public int Level { get; set; }
        public int MasterLevel { get; set; }
        public long Expiration { get; set; }
        public bool Proc { get; set; }
    }
}