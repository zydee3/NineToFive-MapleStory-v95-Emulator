using System.Collections.Generic;
using MapleLib.WzLib;
using NineToFive.Game.Entity.Meta;
using NineToFive.Wz;

namespace NineToFive.Game {
    public class Field {
        int FieldID { get; set; }
        private TemplateField Properties { get; set; }
        
        /// <summary>
        ///     This constructor is meant for when multiple fields are being loaded, we should reuse the loaded WzFile.
        /// </summary>
        public Field() { }

        /// <summary>
        /// This constructor is meant for when only a single field is being loaded so the WzFile being used is a Singleton.
        /// </summary>
        /// <param name="FieldID">Id of the field being loaded</param>
        public Field(int FieldID) {
            MapWz.SetField(this, WzProvider.GetWzProperty(WzProvider.Load("Map"), $"Map/Map${FieldID/100000000}/{FieldID}.img"));
            //todo cache Template 
            Properties = (TemplateField) MapWz.GetTemplateField(FieldID).Clone();
            
        }
        
    }
}