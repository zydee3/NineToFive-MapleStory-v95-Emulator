using System;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game {
    public class SpawnPoint {
        private TemplateLife _life;

        public SpawnPoint(TemplateLife life) {
            _life = life;
        }

        public long LastSummon { get; set; }

        /// <summary>
        /// Summons the mob associated to _mobId if the previous monster has been killed.
        /// </summary>
        public void SummonLife(Field field) {
            Life life = _life.Create();
            if (life == null) return;
            //todo: spawn
            LastSummon = DateTime.Now.ToFileTime();
        }
    }
}