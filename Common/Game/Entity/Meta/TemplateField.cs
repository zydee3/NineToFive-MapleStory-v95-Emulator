using System;
using System.Collections.Generic;
using NineToFive.Constants;

namespace NineToFive.Game.Entity.Meta {
    /// <summary>
    ///     Atomic Variables don't exist in c#, so we must use System.Threading.Interlock for multi-threading.
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.increment?view=netcore-3.1
    /// </summary>
    public class TemplateField {
        public TemplateField(int fieldId) {
            FieldId = fieldId;
            Life = new Dictionary<EntityType, List<TemplateLife>>();
            foreach (EntityType type in Enum.GetValues(typeof(EntityType))) {
                Life.Add(type, new List<TemplateLife>());
            }
        }

        public int FieldId { get; }
        public uint FieldLimit { get; set; }
        public Dictionary<EntityType, List<TemplateLife>> Life { get; }
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
    }
}