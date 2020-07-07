using System.Numerics;

namespace NineToFive.Game.Entity {
    public class Life {
        protected Life(EntityType entityType) {
            EntityType = entityType;
            Location = new Vector2();
            Velocity = new Vector2();
        }

        protected Life(int id, EntityType entityType) : this(entityType) {
            Id = id;
        }

        public int Id { get; set; }
        public uint PoolId { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        public EntityType EntityType { get; }
    }
}