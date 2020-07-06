using NineToFive.Constants;

namespace NineToFive.Game.Entity {
    public class Pet : Life {
        private int ID { get; set; }
        
        public Pet(int ID, int itemId) : base(EntityType.Pet) {
            this.ID = ID;
        }
    }
}