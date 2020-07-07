using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NineToFive.Game.Entity;
using NineToFive.Util;
using NineToFive.Wz;

namespace NineToFive.Game {
    /// <summary>
    /// Properties contains the common properties that all instances of the same field should have so I didn't make a
    /// full copy of them over because it would just be redundant data stored. Because it's immutable, all instances
    /// of the same field can access it to get the properties.
    ///
    /// Life should only hold monsters that are alive / custom entities (ex: entities spawned specific to this instance)
    /// </summary>
    public class Field : PacketBroadcaster {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Field));

        public Field(int id, uint channelId) {
            Id = id;
            ChannelId = channelId;

            LifePools = new Dictionary<EntityType, LifePool<Life>>();
            SpawnPoints = new List<SpawnPoint>();
            Portals = new List<Portal>();
            foreach (EntityType type in Enum.GetValues(typeof(EntityType))) {
                LifePools.Add(type, new LifePool<Life>());
            }

            MapWz.SetField(this);
        }

        public int Id { get; }
        public uint ChannelId { get; }
        public Foothold[] Footholds { get; set; }
        public List<Portal> Portals { get; }
        public string BackgroundMusic { get; set; }
        public string OnFirstUserEnter { get; set; }
        public string OnUserEnter { get; set; }
        public int ForcedReturn { get; set; }
        public int ReturnMap { get; set; }
        public bool Town { get; set; }
        public bool Swim { get; set; }
        public bool Fly { get; set; }
        public int VRTop { get; set; }
        public int VRBottom { get; set; }
        public int VRLeft { get; set; }
        public int VRRight { get; set; }
        public int MobCount { get; set; }
        public float MobRate { get; set; }
        public List<SpawnPoint> SpawnPoints { get; }
        public Dictionary<EntityType, LifePool<Life>> LifePools { get; }
        public bool[] FieldLimits { get; set; }

        /// <summary>
        /// Calculates the projected position on a foothold directly under the provided argument.
        /// </summary>
        /// <param name="position">Position reference to find point on ground underneath.</param>
        /// <returns>Position as tuple(item1=x, item2=y)</returns>
        public Tuple<int, int> GetGroundBelow(Tuple<int, int> position) {
            int smallestYDistance = 999999;
            Foothold foundFoothold = null;
            foreach (Foothold foothold in Footholds) {
                (int X, int Y) = position;
                if (foothold.LeftEndPoint.Item1 <= X && foothold.RightEndPoint.Item1 >= X) {
                    int distanceFromUpperY = Y - Math.Max(foothold.Y1, foothold.Y2);
                    int distanceFromLowerY = Y - Math.Min(foothold.Y1, foothold.Y2);

                    if (distanceFromUpperY >= 0 && distanceFromUpperY < smallestYDistance) {
                        smallestYDistance = distanceFromUpperY;
                        foundFoothold = foothold;
                    } else if (distanceFromLowerY >= 0 && distanceFromLowerY < smallestYDistance) {
                        smallestYDistance = distanceFromLowerY;
                        foundFoothold = foothold;
                    }
                }
            }

            return new Tuple<int, int>(position.Item1, foundFoothold.SlopeForm.GetYLocation(position.Item1));
        }

        public override IEnumerable<Client> GetClients() {
            return LifePools[EntityType.Player].Values.Cast<User>().Select(u => u.Client);
        }

        public T GetLife<T>(EntityType type, uint uniqueId) where T : Life {
            T life = LifePools[type][uniqueId] as T;
            if (life?.EntityType != type) throw new InvalidOperationException($"Type mismatch. Entity is {life.EntityType} but {type} is specified");
            return life;
        }

        public void AddLife(Life life) {
            if (life.PoolId != 0) {
                Log.Info("Life already exists in a field");
                return;
            }

            if (life is User user) {
                user.Field = this;
                // todo broadcast OnEnterField
            }

            LifePools[life.EntityType].AddLife(life);
        }

        public void RemoveLife(Life life) {
            if (!LifePools[life.EntityType].RemoveLife(life)) return;
            life.PoolId = 0;

            if (life is User user) {
                user.Field = null;
                if (LifePools[EntityType.Player].Count == 0) {
                    Log.Info($"There are no more players in field {Id} channel {user.Client.Channel}");
                }

                // todo broadcast OnLeaveField
            }
        }
    }
}