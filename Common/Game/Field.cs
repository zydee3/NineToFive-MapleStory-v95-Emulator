using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using NineToFive.Game.Entity.Meta;
using NineToFive.Wz;

namespace NineToFive.Game {
    public class Field {
        public uint ID { get; set; }
        public uint ChannelID { get; set; }
        public TemplateField Properties { get; set; }
        
        /// <summary>
        ///     This constructor is meant for when multiple fields are being loaded, we should reuse the loaded WzFile.
        /// </summary>
        public Field() { }

        /// <summary>
        /// This constructor is meant for when only a single field is being loaded so the WzFile being used is a Singleton.
        /// </summary>
        /// <param name="FieldID">Id of the field being loaded</param>
        public Field(uint ID, uint ChannelID) {
            this.ID = ID;
            this.ChannelID = ChannelID;
            
            string PathToMapImage = $"Map/Map{ID/100000000}/{ID}.img";
            List<WzImageProperty> FieldProperties = WzProvider.GetWzProperties(WzProvider.Load("Map"), PathToMapImage);
            MapWz.SetField(this, ref FieldProperties);
        }
    }
}