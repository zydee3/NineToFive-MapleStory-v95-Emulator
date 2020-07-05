using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using MapleLib.WzLib.WzStructure.Data;
using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Wz;


namespace NineToFive.Game {
    /// <summary>
    /// Properties contains the common properties that all instances of the same field should have so I didn't make a
    /// full copy of them over because it would just be redundant data stored. Because it's immutable, all instances
    /// of the same field can access it to get the properties.
    ///
    /// Life should only hold monsters that are alive / custom entities (ex: entities spawned specific to this instance)
    /// </summary>
    public class Field {
        public int Id { get; set; }
        public uint ChannelId { get; }
        
        public Foothold[] Footholds { get; set; }
        public Portal[] Portals;
        
        public string BackgroundMusic { get; set; }
        public string OnFirstUserEnter { get; set; }
        public string OnUserEnter { get; set; }
        
        public int ForcedReturn { get; set; }
        public int ReturnMap { get; set; }
        
        public bool Town { get; set; }
        public bool Swim { get; set; }
        public bool Fly  { get; set; }
        
        public int VRTop { get; set; }
        public int VRBottom { get; set; }
        public int VRLeft { get; set; }
        public int VRRight { get; set; }
        
        public int MobCount  { get; set; }
        public float MobRate { get; set; }

        public List<SpawnPoint> SpawnPoints { get; set; } = new List<SpawnPoint>();
        public Dictionary<EntityType, Dictionary<int, Entity.Meta.Entity>> Life { get; set; }
        public bool[] FieldLimits { get; set; }
        
        /// <summary>
        /// Field Constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChannelID"></param>
        public Field(int id, uint channelId) {
            Id = id;
            ChannelId = channelId;
            MapWz.SetField(this);
        }

        /// <summary>
        /// Calculates the projected position on a foothold directly under the provided argument.
        /// </summary>
        /// <param name="Position">Position reference to find point on ground underneath.</param>
        /// <returns>Position as tuple(item1=x, item2=y)</returns>
        public Tuple<int, int> GetGroundBelow(Tuple<int, int> Position) {
            int SmallestYDistance = 999999;
            Foothold FoundFoothold = null;
            foreach (Foothold Foothold in Footholds) {
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