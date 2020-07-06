using NineToFive.Game.Entity;

namespace NineToFive.Game {
    public class SpawnPoint {
        private readonly Field _field;

        public SpawnPoint(Field field, int mobId) {
            _field = field;
            MobId = mobId;
        }

        private int MobId { get; set; }
        public Mob Mob { get; set; }

        public void SummonMob() {
            if (Mob != null) return;

            Mob = new Mob(MobId);
            _field.AddLife(Mob);

            //todo: spawn
        }
    }
}