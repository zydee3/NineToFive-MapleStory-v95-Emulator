using System;
using System.Threading.Tasks;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game {
    public class SpawnPoint {
        private readonly TemplateLife _life;
        private readonly Field _field;
        public bool CanSpawn { get; set; }

        public SpawnPoint(ref Field field, TemplateLife life) {
            _field = field;
            _life = life;
            CanSpawn = true;
        }

        public long NextSummon { get; set; }

        /// <summary>
        /// Summons the mob associated to _mobId if the previous monster has been killed.
        /// </summary>
        public async Task SummonMob(User controller, long currentTime) {
            if (controller == null 
                || !CanSpawn
                || _field.SpawnedMobCount > _field.SpawnedMobLimit
                || currentTime < NextSummon) 
                return;

            Mob mob = (Mob) _life.Create();
            if (mob != null) {
                _field.AddLife(mob);
                _field.BroadcastPacket(mob.EnterFieldPacket());

                mob.SpawnPoint = this;
                mob.UpdateController(controller);

                _field.LastUpdate = currentTime;
                CanSpawn = false;
            }
        }
    }
}