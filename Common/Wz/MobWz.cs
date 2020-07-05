using System;
using System.Collections.Generic;
using System.Linq;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public class MobWz {
        public static void SetMob(Mob mob) {
            if (mob == null) return;
            
            Dictionary<int, object> TemplateMobs = Server.Worlds[0].Templates[(int) TemplateType.Mob];
            if (!TemplateMobs.TryGetValue(mob.Id, out object t)) {
                string PathToMobImage = mob.Id.ToString().PadLeft(7, '0');
                List<WzImageProperty> MobProperties = WzProvider.GetWzProperties(WzProvider.Load("Mob"), $"{PathToMobImage}.img");
                t = new TemplateMob();
                SetTemplateMob((TemplateMob) t, ref MobProperties);
                TemplateMobs.Add(mob.Id, t);
            }

            if (t == null) return;

            TemplateMob template = (TemplateMob) t;
            
            mob.BodyAttack = template.BodyAttack;
            mob.Level = template.Level;
            mob.MaxHP = template.MaxHP;
            mob.MaxMP = template.MaxMP;
            mob.Speed = template.Speed;
            mob.PADamage = template.PADamage;
            mob.PDDamage = template.PDDamage;
            mob.PDRate = template.PDRate;
            mob.MADamage = template.MADamage;
            mob.MDDamage = template.MDDamage;
            mob.MDRate = template.MDRate;
            mob.Acc = template.Acc;
            mob.Eva = template.Eva;
            mob.Pushed = template.Pushed;
            mob.SummonType = template.SummonType;
            mob.Boss = template.Boss;
            mob.IgnoreFieldOut = template.IgnoreFieldOut;
            mob.Category = template.Category;
            mob.HPgaugeHide = template.HPgaugeHide;
            mob.HpTagColor = template.HpTagColor;
            mob.HpTagBgColor = template.HpTagBgColor;
            mob.FirstAttack = template.FirstAttack;
            mob.Exp = template.Exp;
            mob.HpRecovery = template.HpRecovery;
            mob.MpRecovery = template.MpRecovery;
            mob.ExplosiveReward = template.ExplosiveReward;
            mob.HideName = template.HideName;
            mob.RemoveAfter = template.RemoveAfter;
            mob.NoFlip = template.NoFlip;
            mob.Undead = template.Undead;
            mob.DamagedByMob = template.DamagedByMob;
            mob.RareItemDropLevel = template.RareItemDropLevel;
            mob.FlySpeed = template.FlySpeed;
            mob.PublicReward = template.PublicReward;
            mob.Invincible = template.Invincible;
            mob.UpperMostLayer = template.UpperMostLayer;
            mob.NoRegen = template.NoRegen;
            mob.HideHP = template.HideHP;
            mob.MBookID = template.MBookID;
            mob.NoDoom = template.NoDoom;
            mob.FixedDamage = template.FixedDamage;
            mob.RemoveQuest = template.RemoveQuest;
            mob.ChargeCount = template.ChargeCount;
            mob.AngerGauge = template.AngerGauge;
            mob.ChaseSpeed = template.ChaseSpeed;
            mob.Escort = template.Escort;
            mob.RemoveOnMiss = template.RemoveOnMiss;
            mob.CoolDamage = template.CoolDamage;
            mob.CoolDamageProb = template.CoolDamageProb;
            mob._0 = template._0;
            mob.GetCP = template.GetCP;
            mob.CannotEvade = template.CannotEvade;
            mob.DropItemPeriod = template.DropItemPeriod;
            mob.OnlyNormalAttack = template.OnlyNormalAttack;
            mob.Point = template.Point;
            mob.FixDamage = template.FixDamage;
            mob.Weapon = template.Weapon;
            mob.NotAttack = template.NotAttack;
            mob.DoNotRemove = template.DoNotRemove;
            mob.CantPassByTeleport = template.CantPassByTeleport;
            mob.Phase = template.Phase;
            mob.DualGauge = template.DualGauge;
            mob.Disable = template.Disable;

            mob.Fs = template.Fs;

            mob.PartyReward = template.PartyReward;
            mob.Buff = template.Buff;
            mob.DefaultHP = template.DefaultHP;
            mob.DefaultMP = template.DefaultMP;
            mob.Link = template.Link;
            mob.MobType = template.MobType;
            mob.ElemAttr = template.ElemAttr;

            mob.MonsterBan = template.MonsterBan;
            mob.HealOnDestroy = template.HealOnDestroy;
            mob.SelfDestruction = mob.SelfDestruction;
            
            mob.Revives = template.Revives == null ? new int[0] : template.Revives.ToArray();
            mob.Skills = template.Skills == null ? new TemplateMob.Skill[0] : template.Skills.ToArray();
            mob.LoseItems = template.LoseItems == null ? new TemplateMob.LoseItem[0] : template.LoseItems.ToArray();
            mob.DamagedBySelectedMob = template.DamagedBySelectedMob == null ? new int[0] : template.DamagedBySelectedMob.ToArray();
            mob.DamagedBySelectedSkill = template.DamagedBySelectedSkill == null ? new int[0] : template.DamagedBySelectedSkill.ToArray();
        }

        public static void SetTemplateMob(TemplateMob template, ref List<WzImageProperty> mobProperties) {
            foreach (WzImageProperty Node in mobProperties) {
                if (Node.Name != "info") continue;
                foreach (WzImageProperty Property in Node.WzProperties) {
                    switch (Property.Name) {
                        case "level":
                            template.Level = ((WzIntProperty) Property).Value;
                            break;
                        case "exp":
                            template.Exp = ((WzIntProperty) Property).Value;
                            break;
                        case "hpRecovery":
                            template.HpRecovery = ((WzIntProperty) Property).Value;
                            break;
                        case "mpRecovery":
                            template.MpRecovery = ((WzIntProperty) Property).Value;
                            break;    
                        case "maxHP":
                            template.MaxHP = ((WzIntProperty) Property).Value;
                            break;
                        case "maxMP":
                            template.MaxMP = ((WzIntProperty) Property).Value;
                            break;
                        case "defaultHP":
                            template.DefaultHP = ((WzStringProperty) Property).Value;
                            break;
                        case "defaultMP":
                            template.DefaultMP = ((WzStringProperty) Property).Value;
                            break;
                        case "speed":
                            template.Speed = ((WzIntProperty) Property).Value;
                            break;
                        case "PADamage":
                            template.PADamage = ((WzIntProperty) Property).Value;
                            break;
                        case "PDDamage":
                            template.PDDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "PDRate":
                            template.PDRate = ((WzIntProperty) Property).Value;
                            break;
                        case "MADamage":
                            template.MADamage = ((WzIntProperty) Property).Value;
                            break;
                        case "MDDamage":
                            template.MDDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "MDRate":
                            template.MDRate = ((WzIntProperty) Property).Value;
                            break;
                        case "acc":
                            template.Acc = ((WzIntProperty) Property).Value;
                            break;
                        case "eva":
                            template.Eva = ((WzIntProperty) Property).Value;
                            break;
                        
                        
                        case "bodyattack":
                        case "bodyAttack":
                            template.BodyAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "pushed":
                            template.Pushed = ((WzIntProperty) Property).Value;
                            break;
                        case "fs":
                            template.Fs = ((WzFloatProperty) Property).Value;
                            break;
                        case "summonType":
                            template.SummonType = ((WzIntProperty) Property).Value;
                            break;
                        case "boss":
                            template.Boss = ((WzIntProperty) Property).Value;
                            break;
                        case "ignoreFieldOut":
                            template.IgnoreFieldOut = ((WzIntProperty) Property).Value;
                            break;
                        case "elemAttr":
                            template.ElemAttr = ((WzStringProperty) Property).Value;
                            break;
                        case "category":
                            template.Category = ((WzIntProperty) Property).Value;
                            break;
                        case "mobType":
                            template.MobType = ((WzStringProperty) Property).Value;
                            break;
                        case "HPgaugeHide":
                            template.HPgaugeHide = ((WzIntProperty) Property).Value;
                            break;
                        case "hpTagColor":
                            template.HpTagColor = ((WzIntProperty) Property).Value;
                            break;
                        case "hpTagBgcolor":
                            template.HpTagBgColor = ((WzIntProperty) Property).Value;
                            break;
                        case "firstattack":
                        case "firstAttack":
                            template.FirstAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "explosiveReward":
                            template.ExplosiveReward = ((WzIntProperty) Property).Value;
                            break;
                        case "link":
                            template.Link = ((WzStringProperty) Property).Value;
                            break;
                        case "hidename":
                        case "hideName":
                            template.HideName = ((WzIntProperty) Property).Value;
                            break;
                        case "removeAfter":
                            template.RemoveAfter = ((WzIntProperty) Property).Value;
                            break;
                        case "noFlip":
                            template.NoFlip = ((WzIntProperty) Property).Value;
                            break;
                        case "undead":
                            template.Undead = ((WzIntProperty) Property).Value;
                            break;
                        case "damagedByMob":
                            template.DamagedByMob = ((WzIntProperty) Property).Value;
                            break;
                        case "rareItemDropLevel":
                            template.RareItemDropLevel = ((WzIntProperty) Property).Value;
                            break;
                        case "flySpeed":
                            template.FlySpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "publicReward":
                            template.PublicReward = ((WzIntProperty) Property).Value;
                            break;
                        case "invincible":
                            template.Invincible = ((WzIntProperty) Property).Value;
                            break;
                        case "upperMostLayer":
                            template.UpperMostLayer = ((WzIntProperty) Property).Value;
                            break;
      
                        case "PartyReward":
                            template.PartyReward = ((WzStringProperty) Property).Value;
                            break;
                        case "noregen":
                            template.NoRegen = ((WzIntProperty) Property).Value;
                            break;
                        case "hideHP":
                            template.HideHP = ((WzIntProperty) Property).Value;
                            break;
                        case "mbookID":
                            template.MBookID = ((WzIntProperty) Property).Value;
                            break;
                        case "noDoom":
                            template.NoDoom = ((WzIntProperty) Property).Value;
                            break;
                        case "fixedDamage":
                            template.FixedDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "removeQuest":
                            template.RemoveQuest = ((WzIntProperty) Property).Value;
                            break;
                        case "ChargeCount":
                            template.ChargeCount = ((WzIntProperty) Property).Value;
                            break;
                        case "AngerGauge":
                            template.AngerGauge = ((WzIntProperty) Property).Value;
                            break;
                        case "chaseSpeed":
                            template.ChaseSpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "escort":
                            template.Escort = ((WzIntProperty) Property).Value;
                            break;
                        case "removeOnMiss":
                            template.RemoveOnMiss = ((WzIntProperty) Property).Value;
                            break;
                        case "coolDamage":
                            template.CoolDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "coolDamageProb":
                            template.CoolDamageProb = ((WzIntProperty) Property).Value;
                            break;
                        case "0":
                            template._0 = ((WzIntProperty) Property).Value;
                            break;
                        case "getCP":
                            template.GetCP = ((WzIntProperty) Property).Value;
                            break;
                        case "cannotEvade":
                            template.CannotEvade = ((WzIntProperty) Property).Value;
                            break;
                        case "dropItemPeriod":
                            template.DropItemPeriod = ((WzIntProperty) Property).Value;
                            break;
                        case "onlyNormalAttack":
                            template.OnlyNormalAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "point":
                            template.Point = ((WzIntProperty) Property).Value;
                            break;
                        case "fixDamage":
                            template.FixDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "weapon":
                            template.Weapon = ((WzIntProperty) Property).Value;
                            break;
                        case "notAttack":
                            template.NotAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "doNotRemove":
                            template.DoNotRemove = ((WzIntProperty) Property).Value;
                            break;
                        case "buff":
                            template.Buff = ((WzStringProperty) Property).Value;
                            break;
                        case "Speed":
                            template.Speed = ((WzIntProperty) Property).Value;
                            break;
                        case "cantPassByTeleport":
                            template.CantPassByTeleport = ((WzIntProperty) Property).Value;
                            break;
                        case "phase":
                            template.Phase = ((WzIntProperty) Property).Value;
                            break;
                        case "flyspeed":
                        case "FlySpeed":
                            template.FlySpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "dualGauge":
                            template.DualGauge = ((WzIntProperty) Property).Value;
                            break;
                        case "disable":
                            template.Disable = ((WzIntProperty) Property).Value;
                            break;
                        
                        case "ban":
                            template.MonsterBan = new TemplateMob.Ban();
                            foreach (WzImageProperty BanProperty in Property.WzProperties) {
                                switch (BanProperty.Name) {
                                    case "0":
                                        foreach (WzImageProperty TargetProperty in BanProperty.WzProperties) {
                                            switch (TargetProperty.Name) {
                                                case "field":
                                                    template.MonsterBan.TargetFieldID = ((WzIntProperty) TargetProperty).Value;
                                                    break;
                                                case "portal":
                                                    template.MonsterBan.TargetPortalName = ((WzStringProperty) TargetProperty).Value;
                                                    break;
                                                default:
                                                    Console.WriteLine($"Unhandled Ban Target Property: {BanProperty.Name} ({BanProperty.GetType()})");
                                                    break;
                                                    
                                            }
                                        }
                                        break;
                                    case "banMsg":
                                        template.MonsterBan.Message = ((WzStringProperty) BanProperty).Value;
                                        break;
                                    case "banMsgType":
                                        template.MonsterBan.MessageType = ((WzIntProperty) BanProperty).Value;
                                        break;
                                    case "banType":
                                        template.MonsterBan.Type = ((WzIntProperty) BanProperty).Value;
                                        break;
                                    default:
                                        Console.WriteLine($"Unhandled Ban Message Property: {BanProperty.Name} ({BanProperty.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "loseItem":
                            template.LoseItems = new List<TemplateMob.LoseItem>();
                            foreach (WzImageProperty Items in Property.WzProperties) {
                                TemplateMob.LoseItem LoseItem = new TemplateMob.LoseItem();
                                foreach (WzImageProperty LoseProperty in Items.WzProperties) {
                                    switch (LoseProperty.Name) {
                                        case "id":
                                            LoseItem.ID = ((WzIntProperty) LoseProperty).Value;
                                            break;
                                        case "loseMsg":
                                            LoseItem.Message = ((WzStringProperty) LoseProperty).Value;
                                            break;
                                        case "loseMsgType":
                                            LoseItem.MessageType = ((WzIntProperty) LoseProperty).Value;
                                            break;
                                        case "notDrop":
                                            LoseItem.Drop = ((WzIntProperty) LoseProperty).Value == 1; 
                                            break;
                                        case "prop":
                                            LoseItem.Prop = ((WzIntProperty) LoseProperty).Value;
                                            break;
                                        case "x":
                                            LoseItem.X = ((WzIntProperty) LoseProperty).Value;
                                            break;
                                        default:
                                            Console.WriteLine($"Unhandled Lose Item Property: {LoseProperty.Name} ({LoseProperty.GetType()})");
                                            break;
                                    }
                                }
                            }
                            break;
                        case "damagedBySelectedMob":
                            template.DamagedBySelectedMob = new List<int>();
                            foreach (WzImageProperty Mob in Property.WzProperties) {
                                template.DamagedBySelectedMob.Add(((WzIntProperty)Mob).Value);
                            }
                            break;
                        case "damagedBySelectedSkill":
                            template.DamagedBySelectedSkill = new List<int>();
                            foreach (WzImageProperty Skill in Property.WzProperties) {
                                template.DamagedBySelectedSkill.Add(((WzIntProperty)Skill).Value);
                            }
                            break;
                        case "healOnDestroy":
                            int? Type = null, Amount = null;
                            foreach (WzImageProperty Heal in Property.WzProperties) {
                                switch (Heal.Name) {
                                    case "amount":
                                        Amount = ((WzIntProperty) Heal).Value;
                                        break;
                                    case "type":
                                        Type = ((WzIntProperty) Heal).Value;
                                        break;
                                    default:
                                        Console.WriteLine($"Unhandled HealOnDestroy Property: {Heal.Name} ({Heal.GetType()})");
                                        break;
                                }
                            }

                            if (Amount.HasValue && Type.HasValue) {
                                template.HealOnDestroy = new Tuple<int, int>(Type.Value, Amount.Value);
                            }
                            break;
                        case "selfDestruction":
                            foreach (WzImageProperty Destruction in Property.WzProperties) {
                                switch (Destruction.Name) {
                                    case "action":
                                        template.SelfDestruction = ((WzIntProperty) Destruction).Value;
                                        break;
                                    default:
                                        Console.WriteLine($"Unhandled SelfDestruction Property: {Destruction.Name} ({Destruction.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "skill":
                            template.Skills = new List<TemplateMob.Skill>();
                            foreach (WzImageProperty MonsterSkillLevel in Property.WzProperties) {
                                TemplateMob.Skill Skill = new TemplateMob.Skill();
                                foreach (WzImageProperty MonsterSkill in MonsterSkillLevel.WzProperties) {
                                    switch (MonsterSkill.Name) {
                                        case "action":
                                            Skill.Action = ((WzIntProperty) MonsterSkill).Value;
                                            break;
                                        case "effectAfter":
                                            Skill.EffectAfter = ((WzIntProperty) MonsterSkill).Value;
                                            break;
                                        case "level":
                                            Skill.Level = ((WzIntProperty) MonsterSkill).Value;
                                            break;
                                        case "skill":
                                            Skill.ID = ((WzIntProperty) MonsterSkill).Value;
                                            break;
                                        case "skillAfter":
                                            Skill.NextID = ((WzIntProperty) MonsterSkill).Value;
                                            break;
                                        default:
                                            Console.WriteLine($"Unhandled MonsterSkill Property: {MonsterSkill.Name} ({MonsterSkill.GetType()})");
                                            break;
                                    }
                                }
                                template.Skills.Add(Skill);
                            }
                            break;
                        case "revive":
                            template.Revives = new List<int>();
                            foreach (WzImageProperty Revive in Property.WzProperties) {
                                template.Revives.Add(((WzIntProperty)Revive).Value);
                            }
                            break;
                        case "speak":
                            break;
                        case "default":
                            break;
                        default:
                            Console.WriteLine($"Unhandled Mob Property: {Property.Name}");
                            break;
                    }
                }
                
            }

        }

        public static void PrintDirectory() {
            List<string> UniqueProperties = new List<string>();
            foreach (WzImage Image in WzProvider.Load("Mob").WzDirectory.WzImages) {
                foreach (WzImageProperty Info in Image.WzProperties.Where(Property => Property.Name == "info")) {
                    foreach (WzImageProperty Property in Info.WzProperties) {
                        if (UniqueProperties.All(Name => Name != Property.Name)) {
                            UniqueProperties.Add(Property.Name);
                            //Console.WriteLine($"{Property.Name, 25} = {Property.GetType()}, from ({Image.Name})");
                            Console.WriteLine($"case \"{Property.Name}\":\nbreak;");
                        }
                    }
                }
            }
        }
    }
}