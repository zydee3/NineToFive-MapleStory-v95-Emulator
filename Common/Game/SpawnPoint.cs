using System;
using System.Threading.Tasks;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Util;

namespace NineToFive.Game {
    public class SpawnPoint {
        private readonly Field _field;
        private readonly TemplateLife _life;

        public SpawnPoint(ref Field field, TemplateLife life) {
            _field = field;
            _life = life;
        }

        public bool CanSpawn => Enabled && _field.SpawnedMobCount < _field.SpawnedMobLimit && Time.GetCurrent() >= NextSummon;
        public long NextSummon { get; set; }
        public bool Enabled { get; set; } = true;

        public async Task SummonMob() {
            var now = Time.GetCurrent();
            if (!CanSpawn) return;

            var mob = (Mob) _life.Create();
            mob.Death += m => {
                NextSummon = Time.GetFuture(_field.SpawnMobInterval);
                Enabled = true;
            };

            _field.AddLife(mob);
            _field.BroadcastPacket(mob.EnterFieldPacket());

            Enabled = false;
            _field.LastUpdate = now;
        }
    }
}