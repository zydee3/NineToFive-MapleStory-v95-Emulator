using System;
using System.Numerics;
using NineToFive.Constants;

namespace NineToFive.Game.Entity {
    public class Life : IDisposable {
        protected Life(EntityType type) {
            Type = type;
            Location = new Vector2();
            Velocity = new Vector2();
        }

        protected Life(int id, EntityType entityType) : this(entityType) {
            Id = id;
        }

        /// <summary>
        /// a packet which tells the game a life form has been created
        /// </summary>
        public virtual byte[] EnterFieldPacket() {
            throw new InvalidOperationException("Not implemented");
        }

        /// <summary>
        /// a packet which tells the game a life form needs to be removed from the pool
        /// </summary>
        public virtual byte[] LeaveFieldPacket() {
            throw new InvalidOperationException("Not implemented");
        }

        public virtual void Dispose() {
            Field?.RemoveLife(this);
            Field = null;
        }

        public virtual Field Field { get; set; }
        public int Id { get; set; }
        public uint PoolId { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        public int Fh { get; set; }
        public bool Flipped { get; set; }
        public EntityType Type { get; }
    }
}