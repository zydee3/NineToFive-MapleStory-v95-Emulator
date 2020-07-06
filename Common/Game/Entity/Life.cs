using System.Numerics;
using NineToFive.Constants;

namespace NineToFive.Game.Entity {
    public class Life {
        public int Id { get; set; }
        public uint PoolId { get; set; }
        public Vector2 Location { get; set; } = Vector2.Zero;
        public EntityType EntityType { get; }

        protected Life(EntityType entityType) {
            EntityType = entityType;
        }

        protected Life(int id, EntityType entityType) {
            Id = id;
            EntityType = entityType;
        }
    }
}