using System;
using System.Numerics;
using NineToFive.Constants;

namespace NineToFive.Game.Entity.Meta {
    public struct Range2 {
        public readonly int Low;
        public readonly int High;
        public Range2(int low, int high) {
            Low = low;
            High = high;
        }
    }
    /// <summary>
    /// This holds the data from map/life as a template for reference when initializing new fields. 
    /// </summary>
    public class TemplateLife {
        public TemplateLife(EntityType type) {
            Type = type;
        }

        public EntityType Type { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Cy { get; set; }
        public int Rx0 { get; set; }
        public int Rx1 { get; set; }
        public int MobTime { get; set; }
        public int FootholdId { get; set; }
        public bool Flipped { get; set; }
        public bool Hidden { get; set; }
        public int Id { get; set; }

        public Life Create() {
            Life life = Type switch {
                EntityType.Npc     => new Npc(Id) {
                    HorizontalRange = new Range2(Rx0, Rx1)
                },
                EntityType.Mob     => new Mob(Id),
                EntityType.Reactor => new Reactor(Id),
                _                  => throw new InvalidOperationException($"cannot create life of type {Type}")
            };
            life.Fh = FootholdId;
            life.Location = new Vector2(X, Y);
            life.Flipped = Flipped;
            return life;
        }
    }
}