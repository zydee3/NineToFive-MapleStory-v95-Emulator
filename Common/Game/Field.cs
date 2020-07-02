using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using log4net.Util;
using MapleLib.WzLib;
using NineToFive.Game.Entity.Meta;
using NineToFive.Wz;

namespace NineToFive.Game {
    public class Field {
        public uint ID { get; }
        public uint ChannelID { get; }
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

        public Tuple<int, int> GetGroundBelow(Tuple<int, int> Position) {
            int SmallestYDistance = 999999;
            Foothold FoundFoothold = null;
            foreach (Foothold Foothold in Properties.Footholds) {
                (int X, int Y) = Position;
                if (Foothold.LeftEndPoint.Item1 <= X && Foothold.RightEndPoint.Item1 >= X) {
                    int DistanceFromUpperY = Y - Math.Max(Foothold.Y1, Foothold.Y2);
                    int DistanceFromLowerY = Y - Math.Min(Foothold.Y1, Foothold.Y2);

                    if (DistanceFromUpperY >= 0 && DistanceFromUpperY < SmallestYDistance) {
                        SmallestYDistance = DistanceFromUpperY;
                        FoundFoothold = Foothold;
                    } else if (DistanceFromLowerY >= 0 && DistanceFromLowerY < SmallestYDistance) {
                        SmallestYDistance = DistanceFromLowerY;
                        FoundFoothold = Foothold;
                    }
                }
            }

            return new Tuple<int, int>(Position.Item1, FoundFoothold.SlopeForm.GetYLocation(Position.Item1));
        }
        
        
    }
}