using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Packets;
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
        public static readonly int InvalidField = 999999999;

        public Field(int id) {
            Id = id;

            LifePools = new Dictionary<EntityType, LifePool<Life>>();
            SpawnPoints = new List<SpawnPoint>();
            Portals = new List<Portal>();
            foreach (EntityType type in Enum.GetValues(typeof(EntityType))) {
                LifePools.Add(type, new LifePool<Life>(type));
            }

            MapWz.CopyTemplate(this);
        }

        public int Id { get; }
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
        public int VrTop { get; set; }
        public int VrBottom { get; set; }
        public int VrLeft { get; set; }
        public int VrRight { get; set; }
        public int MobCount { get; set; }
        public float MobRate { get; set; }
        public List<SpawnPoint> SpawnPoints { get; }
        public Dictionary<EntityType, LifePool<Life>> LifePools { get; }
        public uint FieldLimit { get; set; }

        /// <summary>
        /// Calculates the projected position on a foothold directly under the provided argument.
        /// </summary>
        /// <param name="position">Position reference to find point on ground underneath.</param>
        /// <returns>Position as tuple(item1=x, item2=y)</returns>
        public Tuple<int, int> GetGroundBelow(Tuple<int, int> position) {
            int smallestYDistance = 999999;
            Foothold foundFoothold = null;
            foreach (Foothold foothold in Footholds) {
                (int x, int y) = position;
                if (foothold.LeftEndPoint.Item1 <= x && foothold.RightEndPoint.Item1 >= x) {
                    int distanceFromUpperY = y - Math.Max(foothold.Y1, foothold.Y2);
                    int distanceFromLowerY = y - Math.Min(foothold.Y1, foothold.Y2);

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
            return LifePools[EntityType.User].Values.Cast<User>().Select(u => u.Client);
        }

        public T GetLife<T>(EntityType type, uint uniqueId) where T : Life {
            T life = LifePools[type][uniqueId] as T;
            if (life != null && life.Type != type) throw new InvalidOperationException($"Type mismatch. Entity is {life.Type} but {type} is specified");
            return life;
        }

        /// <summary>
        /// adds the life to their respective entity pool; silently add the life to the field
        /// </summary>
        public void AddLife(Life life) {
            if (life.Id != 0) {
                Log.Info("Life already exists in a field");
                return;
            }

            life.Field = this;
            LifePools[life.Type].AddLife(life);
        }

        /// <summary>
        /// removes the life from the field if it exists and zeroes the <see cref="Life.Id"/>
        /// <para>Sends the <see cref="Life.LeaveFieldPacket"/> to everybody in the field</para>
        /// </summary>
        public void RemoveLife(Life life) {
            if (!LifePools[life.Type].RemoveLife(life)) return;
            BroadcastPacket(life.LeaveFieldPacket());

            foreach (Mob mob in LifePools[EntityType.Mob].Values) {
                if (life is User target && mob.Controller.Equals(target)) {
                    mob.UpdateController(null);
                }
            }
            
            life.Id = 0;
            life.Field = null;
            if (life is User user) {
                if (LifePools[EntityType.User].Count == 0) {
                    Log.Info($"There are no more players in field {Id}, channel {user.Client.Channel.Id}");
                }
            }

        }

        /// <summary>
        /// summons the life at its defined position; Calls <see cref="AddLife"/> then broadcasts <see cref="Life.EnterFieldPacket"/>
        /// <para>Sends the <see cref="Life.EnterFieldPacket"/> to everybody in the field</para>
        /// </summary>
        public void SummonLife(Life create) {
            AddLife(create);
            var enterFieldPacket = create.Type switch {
                EntityType.Drop => DropPool.GetDropEnterField((Drop) create, 1),
                _               => create.EnterFieldPacket()
            };
            BroadcastPacket(enterFieldPacket);
        }
    }
}