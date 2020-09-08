using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
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
        public const int InvalidField = 999999999;

        private long _lastUpdate;
        private int _spawnedMobCount;
        private int _spawnMobInterval = 5;

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
        public int SpawnedMobLimit { get; set; } = 20;

        public int SpawnedMobCount {
            get => _spawnedMobCount;
            set => Interlocked.Exchange(ref _spawnedMobCount, value);
        }
        
        public int SpawnMobInterval {
            get => _spawnMobInterval;
            set => Interlocked.Exchange(ref _spawnMobInterval, value);
        }

        public long LastUpdate {
            get => _lastUpdate;
            set => Interlocked.Exchange(ref _lastUpdate, value);
        }

        /// <summary>
        /// Each task in this function should be ran asynchronously.
        ///
        /// Each spawn point needs a user for the controller, this serves double to check if a
        /// user exists (since we don't want to update the spawns when no players are in the map)
        /// and as a retrieval so we aren't accessing the life pool on each spawn point.
        ///
        /// I probably shouldn't be passing in channel like this but idk how else to access it.
        /// </summary>
        public async Task Update() {
            foreach (var sp in SpawnPoints) {
                sp.SummonMob().ConfigureAwait(false);
            }

            //todo remove drops, update reactors and etc
            // LifePools[EntityType.Drop].Values.Select(async drop => );
        }

        /// <summary>
        /// Calculates the projected position on a foothold directly under the provided argument.
        /// </summary>
        /// <param name="position">Position reference to find point on ground underneath.</param>
        /// <param name="offset"></param>
        /// <returns>Position as tuple(item1=x, item2=y)</returns>
        public Vector2 GetGroundBelow(Vector2 position, int offset = 0) {
            int smallestRange = int.MaxValue;
            float y = position.Y; // default the y position if new location isn't found.
            bool found = false;   // only add an offset if items
            
            foreach (Foothold foothold in Footholds) {
                if (!foothold.InDomain(position, offset)) continue; // is the current position between the left and right foothold endpoints?
                int distance = foothold.GetRange(position);         // distance vertically between position and the platform; positive if the platform is under position and negative if above
                
                // is this platform closer to the position in comparison to previously found platforms? -70 is for if a platform is close and above, spawn there instead of below if below is far.
                if (distance >= -110 && distance < smallestRange) { 
                    smallestRange = distance;
                    y = foothold.GetYFromX(position.X);
                    found = true;
                }
            }
            
            //bound it so it doesn't spawn outside of the map
            int boundedX = (int) Math.Max(VrLeft + 30, Math.Min(VrRight - 30, position.X + (found ? offset : 0)));
            return new Vector2(boundedX, y);
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
            if (life is User user) {
                if (user.IsHidden) return;
                foreach (var mobs in LifePools[EntityType.Mob].Values) {
                    if (mobs is Mob mob) {
                        if (!mob.Controller.TryGetTarget(out var ctrl) || ctrl.IsHidden) {
                            mob.UpdateController(user);
                        }
                    }
                }
            } else if (life is Mob mob) {
                SpawnedMobCount++;
                var ctrl = LifePools[EntityType.User].Values.Select(l => (User) l).FirstOrDefault();
                mob.UpdateController(ctrl);
            }
        }

        /// <summary>
        /// removes the life from the field if it exists and zeroes the <see cref="Life.Id"/>
        /// <para>Sends the <see cref="Life.LeaveFieldPacket"/> to everybody in the field</para>
        /// </summary>
        public void RemoveLife(Life life) {
            if (!LifePools[life.Type].RemoveLife(life)) return;
            
            life.Id = 0;
            life.Field = null;
            if (life is User user) {
                foreach (var mobs in LifePools[EntityType.Mob].Values) {
                    if (mobs is Mob mob && mob.Controller.TryGetTarget(out var ctrl) && ctrl == user) {
                        mob.UpdateController(null);
                    }
                }

                if (LifePools[EntityType.User].Count == 0) {
                    Log.Info($"There are no more players in field {Id}, channel {user.Client.Channel.Id}");
                }
            } else if (life is Mob mob) {
                SpawnedMobCount--;
                mob.Controller.SetTarget(null);
            }
        }

        /// <summary>
        /// summons the life at its defined position; Calls <see cref="AddLife"/> then broadcasts <see cref="Life.EnterFieldPacket"/>
        /// <para>Sends the <see cref="Life.EnterFieldPacket"/> to everybody in the field</para>
        /// </summary>
        public void SummonLife(Life create) {
            AddLife(create);

            byte[] enterFieldPacket = null;
            switch (create) {
                case Drop drop:
                    enterFieldPacket = DropPool.GetDropEnterField(drop, 1);
                    break;
                case Mob mob:
                    var first = LifePools[EntityType.User].Values.FirstOrDefault();
                    if (first != null && first is User user) mob.UpdateController(user);
                    break;
            }

            enterFieldPacket ??= create.EnterFieldPacket();

            BroadcastPacket(enterFieldPacket);
        }
    }
}