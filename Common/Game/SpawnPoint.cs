using NineToFive.Game.Entity;

namespace NineToFive.Game {
    public class SpawnPoint {
        private readonly Field _field;
        private readonly int _mobId;
        public Mob mob { get; set; }

        public SpawnPoint(Field field, int mobId) {
            _field = field;
            _mobId = mobId;
        }

        /// <summary>
        /// Summons the mob associated to _mobId if the previous monster has been killed.
        /// </summary>
        public void SummonMob() {
            if (mob != null) return;

            mob = new Mob(_mobId);
            //todo: spawn
        }
    }
}