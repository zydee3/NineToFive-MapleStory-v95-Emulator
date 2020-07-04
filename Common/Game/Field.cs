using System;
using System.Collections.Generic;
using MapleLib.WzLib;
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
        public uint ChannelID { get; }
        public TemplateField Properties { get; set; }
        public List<SpawnPoint> SpawnPoints { get; } = new List<SpawnPoint>();
        public Dictionary<EntityType, Dictionary<int, Entity.Meta.Entity>> Life { get; } = new Dictionary<EntityType, Dictionary<int, Entity.Meta.Entity>>();
        
        /// <summary>
        /// Field Constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChannelID"></param>
        public Field(int ID, uint ChannelID) {
            this.ChannelID = ChannelID;
            
            MapWz.SetField(this, ID);
            
            Life = new Dictionary<EntityType, Dictionary<int, Entity.Meta.Entity>>();
            foreach (EntityType Type in Enum.GetValues(typeof(EntityType))) {
                Life.Add(Type, new Dictionary<int, Entity.Meta.Entity>());
            }

            var MobEntries = Properties.Life[EntityType.Mob];
            foreach (Foothold Foothold in Properties.Footholds) {
                
                // Create spawn points only where monsters exist.
                if (MobEntries.TryGetValue(Foothold.ID, out FieldLifeEntry Entry)) {
                    SpawnPoints.Add(new SpawnPoint(this, (int)Entry.ID));
                }
            }
            
            //todo: load npcs and reactors
        }

        /// <summary>
        /// Calculates the projected position on a foothold directly under the provided argument.
        /// </summary>
        /// <param name="Position">Position reference to find point on ground underneath.</param>
        /// <returns>Position as tuple(item1=x, item2=y)</returns>
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