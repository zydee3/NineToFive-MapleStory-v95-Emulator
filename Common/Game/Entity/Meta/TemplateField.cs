using System;
using System.Collections.Generic;
using System.Threading;
using MapleLib.WzLib;
using NineToFive.Constants;
using NineToFive.Wz;


namespace NineToFive.Game.Entity.Meta {
    
    /// <summary>
    ///     Atomic Variables don't exist in c#, so we must use System.Threading.Interlock for multi-threading.
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.increment?view=netcore-3.1
    /// </summary>
    
    public class TemplateField : ICloneable {
        public bool[] FieldLimits { get; set; } = new bool[Enum.GetNames(typeof(FieldLimits)).Length];
        public List<Mob> Life = new List<Mob>();
        public SpawnPoint[] SpawnPoints;
        public Portal[] Portals;
        public bool IsTown { get; set; }
        public int MobCount { get; set; }
        public int ReturnMap { get; set; }
        public double SpawnRate { get; set; }
        public string BackgroundMusic { get; set; }

        public TemplateField() { }
        public TemplateField(WzImageProperty MapImage) {
            MapWz.SetTemplateField(this, MapImage);
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}