using NineToFive.Constants;

namespace NineToFive.Game.Entity.Meta {
    public class Entity {
        public int OID { get; set; }
        public EntityType EntityType { get; set; }

        protected Entity(EntityType EntityType) {
            this.EntityType = EntityType;
        }
        protected Entity(int OID, EntityType EntityType) {
            this.OID = OID;
            this.EntityType = EntityType;
        }
    }
}