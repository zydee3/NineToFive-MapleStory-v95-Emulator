using NineToFive.Constants;
using NineToFive.Game.Entity;


namespace NineToFive.Game {
    public class SpawnPoint {
        private Field Field;
        private int MobID { get; set; }
        public Mob Mob { get; set; }
        public SpawnPoint(Field Field, int MobID) {
            this.Field = Field;
            this.MobID = MobID;
        }

        public void SummonMob() {
            if (Mob != null) return;

            Mob = new Mob(MobID);
            Field.Life[EntityType.Mob].Add((uint)MobID, Mob);
            
            //todo: spawn
        }
    }
}