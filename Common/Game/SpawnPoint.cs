using System;
using System.Threading.Tasks;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game {
    public class SpawnPoint {
        private TemplateLife _life;
        private Field _field;

        public SpawnPoint(ref Field field, TemplateLife life) {
            _field = field;
            _life = life;
        }

        public long LastSummon { get; set; }

        /// <summary>
        /// Summons the mob associated to _mobId if the previous monster has been killed.
        /// </summary>
        public async Task<bool> SummonLife(Field field) {
            Life life = _life.Create();
            if (life == null) return false;
            //todo: spawn
            LastSummon = DateTime.Now.ToFileTime();

            return true;
        }
    }
}