using System;
using System.Numerics;
using System.Threading.Tasks;
using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Packets;
using NineToFive.Resources;
using NineToFive.Util;
using NineToFive.Wz;

namespace NineToFive.Game.Entity {
    public class Mob : Life {
        public delegate void MobDeathHandler(Mob mob);

        private MobDeathHandler _onDeath;
        public Tuple<int, int> HealOnDestroy;
        public int SelfDestruction;

        public Mob(int templateId) : base(templateId, EntityType.Mob) {
            MobWz.SetMob(this);
            ChaseTarget = false;
            Controller = new WeakReference<User>(null);
        }

        public override byte[] EnterFieldPacket() {
            return MobPackets.GetMobEnterField(this);
        }

        public override byte[] LeaveFieldPacket() {
            return MobPackets.GetMobLeaveField(this, 1);
        }

        public WeakReference<User> Controller { get; }
        public bool ChaseTarget { get; set; }

        public void UpdateController(User user, bool chaseTarget = false) {
            Controller.SetTarget(user);
            ChaseTarget = user != null && chaseTarget;
            user?.Client.Session.Write(MobPackets.GetMobChangeController(this));
        }

        public async Task<int> Damage(User attacker, int damage) {
            lock (this) {
                HP -= damage;
                if (HP > 0) {
                    byte indicator = (byte) Math.Ceiling(HP * 100.0 / MaxHP);
                    attacker.Client.Session.Write(MobPackets.GetShowHpIndicator((int) Id, indicator));
                } else {
                    _onDeath(this);
                    attacker.Client.Session.Write(CWvsPackets.GetIncExpMessage(Exp));
                    SpawnDrops();
                    Field.BroadcastPacket(LeaveFieldPacket());
                    Field.RemoveLife(this);
                    return Exp;
                }
            }
    
            return 0;
        }

        public async Task SpawnDrops() {
            int offset = 0, counter = 0;
            Random random = new Random();

            foreach (int dropId in WzCache.MobDrops[TemplateId]) {
                if (random.Next(1, 100) > 20) continue;
                offset = (Math.Abs(offset) + 10) * (counter++ % 2 == 0 ? -1 : 1);
                Vector2 dropLocation = Field.GetGroundBelow(Location, offset);
                Field.SummonLife(new Drop(dropId, 1, Location, dropLocation));
            }
        }

        private int _hp;

        public int HP {
            get => _hp;
            set => _hp = Math.Max(Math.Min(value, MaxHP), 0);
        }

        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int MaxMP { get; set; }
        public int MP { get; set; }
        public int Speed { get; set; }
        public int PADamage { get; set; }
        public int PDDamage { get; set; }
        public int PDRate { get; set; }
        public int MADamage { get; set; }
        public int MDDamage { get; set; }
        public int MDRate { get; set; }
        public int Acc { get; set; }
        public int Eva { get; set; }
        public TemplateMob.Ban MonsterBan { get; set; }
        public TemplateMob.LoseItem[] LoseItems { get; set; }
        public int[] DamagedBySelectedSkill { get; set; }
        public int[] DamagedBySelectedMob { get; set; }
        public int[] Revives { get; set; }
        public TemplateMob.MobSkill[] Skills { get; set; }

        public event MobDeathHandler Death {
            add => _onDeath += value;
            remove => _onDeath -= value;
        }

        public int BodyAttack { get; set; }
        public int Pushed { get; set; }
        public int SummonType { get; set; } = -2;
        public int Boss { get; set; }
        public int IgnoreFieldOut { get; set; }
        public int Category { get; set; }
        public int HPGaugeHide { get; set; }
        public int HpTagColor { get; set; }
        public int HpTagBgColor { get; set; }
        public int FirstAttack { get; set; }
        public int Exp { get; set; }
        public int HpRecovery { get; set; }
        public int MpRecovery { get; set; }
        public int ExplosiveReward { get; set; }
        public int HideName { get; set; }
        public int RemoveAfter { get; set; }
        public int NoFlip { get; set; }
        public int Undead { get; set; }
        public int DamagedByMob { get; set; }
        public int RareItemDropLevel { get; set; }
        public int FlySpeed { get; set; }
        public int PublicReward { get; set; }
        public int Invincible { get; set; }
        public int UpperMostLayer { get; set; }
        public int NoRegen { get; set; }
        public int HideHP { get; set; }
        public int MBookID { get; set; }
        public int NoDoom { get; set; }
        public int FixedDamage { get; set; }
        public int RemoveQuest { get; set; }
        public int ChargeCount { get; set; }
        public int AngerGauge { get; set; }
        public int ChaseSpeed { get; set; }
        public int Escort { get; set; }
        public int RemoveOnMiss { get; set; }
        public int CoolDamage { get; set; }
        public int CoolDamageProb { get; set; }
        public int _0 { get; set; }
        public int GetCP { get; set; }
        public int CannotEvade { get; set; }
        public int DropItemPeriod { get; set; }
        public int OnlyNormalAttack { get; set; }
        public int Point { get; set; }
        public int FixDamage { get; set; }
        public int Weapon { get; set; }
        public int NotAttack { get; set; }
        public int DoNotRemove { get; set; }
        public int CantPassByTeleport { get; set; }
        public int Phase { get; set; }
        public int DualGauge { get; set; }
        public int Disable { get; set; }
        public float Fs { get; set; }
        public string PartyReward { get; set; }
        public string Buff { get; set; }
        public string DefaultHP { get; set; }
        public string DefaultMP { get; set; }
        public string Link { get; set; }
        public string MobType { get; set; }
        public string ElemAttr { get; set; }
    }
}