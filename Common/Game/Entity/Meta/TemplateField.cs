using System;
using System.Collections.Generic;
using MapleLib.WzLib.WzStructure.Data;

namespace NineToFive.Game.Entity.Meta {
    /// <summary>
    ///     Atomic Variables don't exist in c#, so we must use System.Threading.Interlock for multi-threading.
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.increment?view=netcore-3.1
    /// </summary>
    public class TemplateField {
        public TemplateField() {
            Life = new Dictionary<EntityType, Dictionary<int, FieldLifeEntry>>();
            foreach (EntityType Type in Enum.GetValues(typeof(EntityType))) {
                Life.Add(Type, new Dictionary<int, FieldLifeEntry>());
            }
        }

        public bool[] FieldLimits { get; set; } = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
        public Dictionary<EntityType, Dictionary<int, FieldLifeEntry>> Life { get; }

        public Foothold[] Footholds { get; set; }
        public List<Portal> Portals { get; set; }

        public string BackgroundMusic { get; set; }
        public string OnFirstUserEnter { get; set; }
        public string OnUserEnter { get; set; }

        public int ForcedReturn { get; set; }
        public int ReturnMap { get; set; }

        public bool Town { get; set; }
        public bool Swim { get; set; }
        public bool Fly { get; set; }

        public int MobCount { get; set; }
        public float MobRate { get; set; }

        public int VRTop { get; set; }
        public int VRBottom { get; set; }
        public int VRLeft { get; set; }
        public int VRRight { get; set; }

        public bool LoadLife { get; set; } = true;
        public bool LoadClock { get; set; } = true;
        public bool LoadPortals { get; set; } = true;
        public bool LoadReactors { get; set; } = true;
    }
}