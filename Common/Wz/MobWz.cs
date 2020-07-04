using System;
using System.Collections.Generic;
using System.Linq;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.SendOps;

namespace NineToFive.Wz {
    public class MobWz {
        public static void SetMob(Mob Mob, int MobID) {
            if (Mob == null) return;

            Dictionary<int, object> TemplateMobs = Server.Worlds[0].Templates[(int) TemplateType.Mob];
            if (!TemplateMobs.TryGetValue(MobID, out object Template)) {
                string PathToMobImage = MobID.ToString().PadLeft(7, '0');
                List<WzImageProperty> MobProperties = WzProvider.GetWzProperties(WzProvider.Load("Mob"), $"{PathToMobImage}.img");
                Template = new TemplateMob(MobID);
                SetTemplateMob((TemplateMob) Template, ref MobProperties);
            }

            if (Template == null) return;
            Mob.Properties = (TemplateMob) Template;
        }

        public static void SetTemplateMob(TemplateMob Template, ref List<WzImageProperty> MobProperties) {
            foreach (WzImageProperty Node in MobProperties) {
                if (Node.Name != "info") continue;
                foreach (WzImageProperty Property in Node.WzProperties) {
                    switch (Property.Name) {
                        case "level":
                            Template.Level = ((WzIntProperty) Property).Value;
                            break;
                        case "exp":
                            Template.Exp = ((WzIntProperty) Property).Value;
                            break;
                        case "hpRecovery":
                            Template.HpRecovery = ((WzIntProperty) Property).Value;
                            break;
                        case "mpRecovery":
                            Template.MpRecovery = ((WzIntProperty) Property).Value;
                            break;    
                        case "maxHP":
                            Template.MaxHP = ((WzIntProperty) Property).Value;
                            break;
                        case "maxMP":
                            Template.MaxMP = ((WzIntProperty) Property).Value;
                            break;
                        case "defaultHP":
                            Template.DefaultHP = ((WzStringProperty) Property).Value;
                            break;
                        case "defaultMP":
                            Template.DefaultMP = ((WzStringProperty) Property).Value;
                            break;
                        case "speed":
                            Template.Speed = ((WzIntProperty) Property).Value;
                            break;
                        case "PADamage":
                            Template.PADamage = ((WzIntProperty) Property).Value;
                            break;
                        case "PDDamage":
                            Template.PDDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "PDRate":
                            Template.PDRate = ((WzIntProperty) Property).Value;
                            break;
                        case "MADamage":
                            Template.MADamage = ((WzIntProperty) Property).Value;
                            break;
                        case "MDDamage":
                            Template.MDDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "MDRate":
                            Template.MDRate = ((WzIntProperty) Property).Value;
                            break;
                        case "acc":
                            Template.Acc = ((WzIntProperty) Property).Value;
                            break;
                        case "eva":
                            Template.Eva = ((WzIntProperty) Property).Value;
                            break;
                        
                        
                        case "bodyattack":
                        case "bodyAttack":
                            Template.BodyAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "pushed":
                            Template.Pushed = ((WzIntProperty) Property).Value;
                            break;
                        case "fs":
                            Template.Fs = ((WzFloatProperty) Property).Value;
                            break;
                        case "summonType":
                            Template.SummonType = ((WzIntProperty) Property).Value;
                            break;
                        case "boss":
                            Template.Boss = ((WzIntProperty) Property).Value;
                            break;
                        case "ignoreFieldOut":
                            Template.IgnoreFieldOut = ((WzIntProperty) Property).Value;
                            break;
                        case "elemAttr":
                            Template.ElemAttr = ((WzStringProperty) Property).Value;
                            break;
                        case "category":
                            Template.Category = ((WzIntProperty) Property).Value;
                            break;
                        case "mobType":
                            Template.MobType = ((WzStringProperty) Property).Value;
                            break;
                        case "HPgaugeHide":
                            Template.HPgaugeHide = ((WzIntProperty) Property).Value;
                            break;
                        case "hpTagColor":
                            Template.HpTagColor = ((WzIntProperty) Property).Value;
                            break;
                        case "hpTagBgcolor":
                            Template.HpTagBgColor = ((WzIntProperty) Property).Value;
                            break;
                        case "firstattack":
                        case "firstAttack":
                            Template.FirstAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "explosiveReward":
                            Template.ExplosiveReward = ((WzIntProperty) Property).Value;
                            break;
                        case "link":
                            Template.Link = ((WzStringProperty) Property).Value;
                            break;
                        case "hidename":
                        case "hideName":
                            Template.HideName = ((WzIntProperty) Property).Value;
                            break;
                        case "removeAfter":
                            Template.RemoveAfter = ((WzIntProperty) Property).Value;
                            break;
                        case "noFlip":
                            Template.NoFlip = ((WzIntProperty) Property).Value;
                            break;
                        case "undead":
                            Template.Undead = ((WzIntProperty) Property).Value;
                            break;
                        case "damagedByMob":
                            Template.DamagedByMob = ((WzIntProperty) Property).Value;
                            break;
                        case "rareItemDropLevel":
                            Template.RareItemDropLevel = ((WzIntProperty) Property).Value;
                            break;
                        case "flySpeed":
                            Template.FlySpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "publicReward":
                            Template.PublicReward = ((WzIntProperty) Property).Value;
                            break;
                        case "invincible":
                            Template.Invincible = ((WzIntProperty) Property).Value;
                            break;
                        case "upperMostLayer":
                            Template.UpperMostLayer = ((WzIntProperty) Property).Value;
                            break;
      
                        case "PartyReward":
                            Template.PartyReward = ((WzStringProperty) Property).Value;
                            break;
                        case "noregen":
                            Template.NoRegen = ((WzIntProperty) Property).Value;
                            break;
                        case "hideHP":
                            Template.HideHP = ((WzIntProperty) Property).Value;
                            break;
                        case "mbookID":
                            Template.MBookID = ((WzIntProperty) Property).Value;
                            break;
                        case "noDoom":
                            Template.NoDoom = ((WzIntProperty) Property).Value;
                            break;
                        case "fixedDamage":
                            Template.FixedDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "removeQuest":
                            Template.RemoveQuest = ((WzIntProperty) Property).Value;
                            break;
                        case "ChargeCount":
                            Template.ChargeCount = ((WzIntProperty) Property).Value;
                            break;
                        case "AngerGauge":
                            Template.AngerGauge = ((WzIntProperty) Property).Value;
                            break;
                        case "chaseSpeed":
                            Template.ChaseSpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "escort":
                            Template.Escort = ((WzIntProperty) Property).Value;
                            break;
                        case "removeOnMiss":
                            Template.RemoveOnMiss = ((WzIntProperty) Property).Value;
                            break;
                        case "coolDamage":
                            Template.CoolDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "coolDamageProb":
                            Template.CoolDamageProb = ((WzIntProperty) Property).Value;
                            break;
                        case "0":
                            Template._0 = ((WzIntProperty) Property).Value;
                            break;
                        case "getCP":
                            Template.GetCP = ((WzIntProperty) Property).Value;
                            break;
                        case "cannotEvade":
                            Template.CannotEvade = ((WzIntProperty) Property).Value;
                            break;
                        case "dropItemPeriod":
                            Template.DropItemPeriod = ((WzIntProperty) Property).Value;
                            break;
                        case "onlyNormalAttack":
                            Template.OnlyNormalAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "point":
                            Template.Point = ((WzIntProperty) Property).Value;
                            break;
                        case "fixDamage":
                            Template.FixDamage = ((WzIntProperty) Property).Value;
                            break;
                        case "weapon":
                            Template.Weapon = ((WzIntProperty) Property).Value;
                            break;
                        case "notAttack":
                            Template.NotAttack = ((WzIntProperty) Property).Value;
                            break;
                        case "doNotRemove":
                            Template.DoNotRemove = ((WzIntProperty) Property).Value;
                            break;
                        case "buff":
                            Template.Buff = ((WzStringProperty) Property).Value;
                            break;
                        case "Speed":
                            Template.Speed = ((WzIntProperty) Property).Value;
                            break;
                        case "cantPassByTeleport":
                            Template.CantPassByTeleport = ((WzIntProperty) Property).Value;
                            break;
                        case "phase":
                            Template.Phase = ((WzIntProperty) Property).Value;
                            break;
                        case "flyspeed":
                        case "FlySpeed":
                            Template.FlySpeed = ((WzIntProperty) Property).Value;
                            break;
                        case "dualGauge":
                            Template.DualGauge = ((WzIntProperty) Property).Value;
                            break;
                        case "disable":
                            Template.Disable = ((WzIntProperty) Property).Value;
                            break;
                        
                        case "ban":
                            Template.MonsterBan = new TemplateMob.Ban();
                            foreach (WzImageProperty BanProperty in Property.WzProperties) {
                                switch (BanProperty.Name) {
                                    case "0":
                                        foreach (WzImageProperty TargetProperty in BanProperty.WzProperties) {
                                            switch (TargetProperty.Name) {
                                                case "field":
                                                    Template.MonsterBan.TargetFieldID = ((WzIntProperty) TargetProperty).Value;
                                                    break;
                                                case "portal":
                                                    Template.MonsterBan.TargetPortalName = ((WzStringProperty) TargetProperty).Value;
                                                    break;
                                                default:
                                                    Console.WriteLine($"Unhandled Ban Target Property: {BanProperty.Name} ({BanProperty.GetType()})");
                                                    break;
                                                    
                                            }
                                        }
                                        break;
                                    case "banMsg":
                                        Template.MonsterBan.Message = ((WzStringProperty) BanProperty).Value;
                                        break;
                                    case "banMsgType":
                                        Template.MonsterBan.MessageType = ((WzIntProperty) BanProperty).Value;
                                        break;
                                    case "banType":
                                        Template.MonsterBan.Type = ((WzIntProperty) BanProperty).Value;
                                        break;
                                    default:
                                        Console.WriteLine($"Unhandled Ban Message Property: {BanProperty.Name} ({BanProperty.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "loseItem":
                            Template.LoseItems = new List<TemplateMob.LoseItem>();
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
                            Template.DamagedBySelectedMob = new List<int>();
                            foreach (WzImageProperty Mob in Property.WzProperties) {
                                Template.DamagedBySelectedMob.Add(((WzIntProperty)Mob).Value);
                            }
                            break;
                        case "damagedBySelectedSkill":
                            Template.DamagedBySelectedSkill = new List<int>();
                            foreach (WzImageProperty Skill in Property.WzProperties) {
                                Template.DamagedBySelectedSkill.Add(((WzIntProperty)Skill).Value);
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
                                Template.HealOnDestroy = new Tuple<int, int>(Type.Value, Amount.Value);
                            }
                            break;
                        case "selfDestruction":
                            foreach (WzImageProperty Destruction in Property.WzProperties) {
                                switch (Destruction.Name) {
                                    case "action":
                                        Template.SelfDestruction = ((WzIntProperty) Destruction).Value;
                                        break;
                                    default:
                                        Console.WriteLine($"Unhandled SelfDestruction Property: {Destruction.Name} ({Destruction.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "skill":
                            Template.Skills = new List<TemplateMob.Skill>();
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
                                Template.Skills.Add(Skill);
                            }
                            break;
                        case "revive":
                            Template.Revives = new List<int>();
                            foreach (WzImageProperty Revive in Property.WzProperties) {
                                Template.Revives.Add(((WzIntProperty)Revive).Value);
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