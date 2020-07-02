using System;
using System.Collections.Generic;
using System.Threading;
using MapleLib.WzLib;
using MapleLib.WzLib.WzStructure.Data;
using NineToFive.Constants;
using NineToFive.Wz;


namespace NineToFive.Game.Entity.Meta {
    
    /// <summary>
    ///     Atomic Variables don't exist in c#, so we must use System.Threading.Interlock for multi-threading.
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.increment?view=netcore-3.1
    /// </summary>
    
    public class TemplateField : ICloneable {
        public bool[] FieldLimits { get; set; } = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
        public List<Mob> Life = new List<Mob>();
        public SpawnPoint[] SpawnPoints;
        public Portal[] Portals;
        
        public string BackgroundMusic { get; set; }
        public string OnFirstUserEnter { get; set; }
        public string OnUserEnter { get; set; }
        
        public int ForcedReturn { get; set; }
        public int ReturnMap { get; set; }
        
        public bool Town { get; set; }
        public bool Swim { get; set; }
        public bool Fly  { get; set; }
        
        public int MobCount  { get; set; }
        public float MobRate { get; set; }

        public bool LoadReactors { get; set; } = true;
        public bool LoadPortals  { get; set; } = true;
        public bool LoadClock    { get; set; } = true;
        public bool LoadLife { get; set; } = true;
        public Foothold[] Footholds { get; set; }

        public TemplateField() { }
        
        public TemplateField(ref List<WzImageProperty> FieldProperties) {
            MapWz.SetTemplateField(this, ref FieldProperties);
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}