using System;
using System.Collections.Generic;

namespace NineToFive.Game.Entity.Meta {
    public class TemplateMob {

        public class Ban {
            public int Type { get; set; }
            public int TargetFieldID { get; set; }
            public string TargetPortalName { get; set; }
            
            public string Message { get; set; }
            public int MessageType { get; set; }

            public Ban() {
                
            }
        }

        public class LoseItem {
            public int ID { get; set; }
            public string Message { get; set; }
            public int MessageType { get; set; }
            public bool Drop { get; set; }
            public int Prop { get; set; }
            public int X { get; set; }

            public LoseItem() {
                
            }
        }

        public class Skill {
            public int ID { get; set; }
            public int NextID { get; set; }
            public int Action { get; set; }
            public int EffectAfter { get; set; }
            public int Level { get; set; }
        }

        public Ban MonsterBan { get; set; }
        public List<LoseItem> LoseItems { get; set; }
        public List<int> DamagedBySelectedSkill { get; set; }
        public List<int> DamagedBySelectedMob { get; set; }
        public List<int> Revives { get; set; }
        public List<Skill> Skills { get; set; }
        public Tuple<int, int> HealOnDestroy;
        public int SelfDestruction;
        
        public int ID { get; set; }
        
        public int BodyAttack { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int MaxMP { get; set; }
        public int Speed { get; set; }
        public int PADamage { get; set; }
        public int PDDamage { get; set; }
        public int PDRate { get; set; }
        public int MADamage { get; set; }
        public int MDDamage { get; set; }
        public int MDRate { get; set; }
        public int Acc { get; set; }
        public int Eva { get; set; }
        public int Pushed { get; set; }
        public int SummonType { get; set; }
        public int Boss { get; set; }
        public int IgnoreFieldOut { get; set; }
        public int Category { get; set; }
        public int HPgaugeHide { get; set; }
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
        public TemplateMob() {
            
        }

        public TemplateMob(int ID) {
            this.ID = ID;
        }
    }
}

/*
               bodyAttack = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                    level = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                    maxHP = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                    maxMP = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                    speed = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                 PADamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                 PDDamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                   PDRate = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                 MADamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                 MDDamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                   MDRate = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                      acc = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                      eva = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                   pushed = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
               summonType = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                     boss = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
           ignoreFieldOut = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                 category = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
              HPgaugeHide = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
               hpTagColor = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
             hpTagBgcolor = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
              firstAttack = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
                      exp = MapleLib.WzLib.WzProperties.WzIntProperty, from (8300006.img)
               hpRecovery = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300207.img)
               mpRecovery = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300207.img)
          explosiveReward = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300207.img)
                 hideName = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300004.img)
              removeAfter = MapleLib.WzLib.WzProperties.WzIntProperty, from (8810024.img)
                   noFlip = MapleLib.WzLib.WzProperties.WzIntProperty, from (8810024.img)
                   undead = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400334.img)
             damagedByMob = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400334.img)
        rareItemDropLevel = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300289.img)
                 flySpeed = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500117.img)
             publicReward = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400711.img)
               invincible = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400711.img)
           upperMostLayer = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420101.img)
                  noregen = MapleLib.WzLib.WzProperties.WzIntProperty, from (9410015.img)
                   hideHP = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420113.img)
                  mbookID = MapleLib.WzLib.WzProperties.WzIntProperty, from (8810105.img)
                   noDoom = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300278.img)
              fixedDamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500193.img)
              removeQuest = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300389.img)
              ChargeCount = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400633.img)
               AngerGauge = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400633.img)
                 hidename = MapleLib.WzLib.WzProperties.WzIntProperty, from (9302010.img)
               chaseSpeed = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500166.img)
                   escort = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420077.img)
             removeOnMiss = MapleLib.WzLib.WzProperties.WzIntProperty, from (9700018.img)
               coolDamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (9700018.img)
           coolDamageProb = MapleLib.WzLib.WzProperties.WzIntProperty, from (9700018.img)
                        0 = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300365.img)
                    getCP = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300133.img)
              cannotEvade = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420103.img)
           dropItemPeriod = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400707.img)
         onlyNormalAttack = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500320.img)
                    point = MapleLib.WzLib.WzProperties.WzIntProperty, from (9700032.img)
                fixDamage = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300296.img)
                   weapon = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400610.img)
                notAttack = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300018.img)
              firstattack = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420003.img)
              doNotRemove = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300138.img)
                    Speed = MapleLib.WzLib.WzProperties.WzIntProperty, from (9300210.img)
       cantPassByTeleport = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400594.img)
                    phase = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420098.img)
                 flyspeed = MapleLib.WzLib.WzProperties.WzIntProperty, from (9400518.img)
                 FlySpeed = MapleLib.WzLib.WzProperties.WzIntProperty, from (9420506.img)
               bodyattack = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500364.img)
                dualGauge = MapleLib.WzLib.WzProperties.WzIntProperty, from (9500401.img)
                  disable = MapleLib.WzLib.WzProperties.WzIntProperty, from (8830003.img)
                  
                       fs = MapleLib.WzLib.WzProperties.WzFloatProperty, from (8300006.img)
                  
                     link = MapleLib.WzLib.WzProperties.WzStringProperty, from (0100004.img)
                 elemAttr = MapleLib.WzLib.WzProperties.WzStringProperty, from (8300006.img)
                     buff = MapleLib.WzLib.WzProperties.WzStringProperty, from (8820001.img)
              PartyReward = MapleLib.WzLib.WzProperties.WzStringProperty, from (9300139.img)
                defaultHP = MapleLib.WzLib.WzProperties.WzStringProperty, from (9400408.img)
                defaultMP = MapleLib.WzLib.WzProperties.WzStringProperty, from (9400408.img)
                  mobType = MapleLib.WzLib.WzProperties.WzStringProperty, from (8300006.img)
                  
                      ban = MapleLib.WzLib.WzProperties.WzSubProperty, from (9420101.img)
                 loseItem = MapleLib.WzLib.WzProperties.WzSubProperty, from (9420101.img)
   damagedBySelectedSkill = MapleLib.WzLib.WzProperties.WzSubProperty, from (9001005.img)
            healOnDestroy = MapleLib.WzLib.WzProperties.WzSubProperty, from (9420063.img)
          selfDestruction = MapleLib.WzLib.WzProperties.WzSubProperty, from (5100002.img)
     damagedBySelectedMob = MapleLib.WzLib.WzProperties.WzSubProperty, from (9300389.img)
                    speak = MapleLib.WzLib.WzProperties.WzSubProperty, from (9300389.img)
                  default = MapleLib.WzLib.WzProperties.WzSubProperty, from (9300139.img)
                   revive = MapleLib.WzLib.WzProperties.WzSubProperty, from (9300207.img)
                    skill = MapleLib.WzLib.WzProperties.WzSubProperty, from (8300006.img)

*/
