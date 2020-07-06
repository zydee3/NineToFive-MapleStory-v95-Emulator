using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public class MobWz {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MobWz));
        /// <summary>
        /// Copies all the properties from the loaded template in cache to the mob being initialized. If the template
        /// isn't in the cache it is loaded and cached. 
        /// </summary>
        /// <param name="mob">Mob being initialized.</param>
        public static void SetMob(Mob mob) {
            if (mob == null) return;

            int mobId = mob.Id;
            Dictionary<int, object> TemplateMobs = Server.Worlds[0].Templates[(int) TemplateType.Mob];
            if (!TemplateMobs.TryGetValue(mobId, out object t)) {
                string PathToMobImage = mobId.ToString().PadLeft(7, '0');
                List<WzImageProperty> mobProperties = WzProvider.GetWzProperties(WzProvider.Load("Mob"), $"{PathToMobImage}.img");
                t = new TemplateMob();
                SetTemplateMob((TemplateMob) t, ref mobProperties);
                TemplateMobs.Add(mobId, t);
            }

            if (t == null) throw new NullReferenceException($"Unable to load mob: {mobId}");

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

        /// <summary>
        /// This is invoked when a template doesn't exist in the cache. Creates the template to be cached for when creating mirrors of the template.
        /// </summary>
        /// <param name="template">Template being initialized.</param>
        /// <param name="mobProperties">Properties loaded from the wz file.</param>
        public static void SetTemplateMob(TemplateMob template, ref List<WzImageProperty> mobProperties) {
            foreach (WzImageProperty node in mobProperties) {
                if (node.Name != "info") continue;
                foreach (WzImageProperty property in node.WzProperties) {
                    switch (property.Name) {
                        case "level":
                            template.Level = ((WzIntProperty) property).Value;
                            break;
                        case "exp":
                            template.Exp = ((WzIntProperty) property).Value;
                            break;
                        case "hpRecovery":
                            template.HpRecovery = ((WzIntProperty) property).Value;
                            break;
                        case "mpRecovery":
                            template.MpRecovery = ((WzIntProperty) property).Value;
                            break;    
                        case "maxHP":
                            template.MaxHP = ((WzIntProperty) property).Value;
                            break;
                        case "maxMP":
                            template.MaxMP = ((WzIntProperty) property).Value;
                            break;
                        case "defaultHP":
                            template.DefaultHP = ((WzStringProperty) property).Value;
                            break;
                        case "defaultMP":
                            template.DefaultMP = ((WzStringProperty) property).Value;
                            break;
                        case "speed":
                            template.Speed = ((WzIntProperty) property).Value;
                            break;
                        case "PADamage":
                            template.PADamage = ((WzIntProperty) property).Value;
                            break;
                        case "PDDamage":
                            template.PDDamage = ((WzIntProperty) property).Value;
                            break;
                        case "PDRate":
                            template.PDRate = ((WzIntProperty) property).Value;
                            break;
                        case "MADamage":
                            template.MADamage = ((WzIntProperty) property).Value;
                            break;
                        case "MDDamage":
                            template.MDDamage = ((WzIntProperty) property).Value;
                            break;
                        case "MDRate":
                            template.MDRate = ((WzIntProperty) property).Value;
                            break;
                        case "acc":
                            template.Acc = ((WzIntProperty) property).Value;
                            break;
                        case "eva":
                            template.Eva = ((WzIntProperty) property).Value;
                            break;
                        
                        
                        case "bodyattack":
                        case "bodyAttack":
                            template.BodyAttack = ((WzIntProperty) property).Value;
                            break;
                        case "pushed":
                            template.Pushed = ((WzIntProperty) property).Value;
                            break;
                        case "fs":
                            template.Fs = ((WzFloatProperty) property).Value;
                            break;
                        case "summonType":
                            template.SummonType = ((WzIntProperty) property).Value;
                            break;
                        case "boss":
                            template.Boss = ((WzIntProperty) property).Value;
                            break;
                        case "ignoreFieldOut":
                            template.IgnoreFieldOut = ((WzIntProperty) property).Value;
                            break;
                        case "elemAttr":
                            template.ElemAttr = ((WzStringProperty) property).Value;
                            break;
                        case "category":
                            template.Category = ((WzIntProperty) property).Value;
                            break;
                        case "mobType":
                            template.MobType = ((WzStringProperty) property).Value;
                            break;
                        case "HPgaugeHide":
                            template.HPgaugeHide = ((WzIntProperty) property).Value;
                            break;
                        case "hpTagColor":
                            template.HpTagColor = ((WzIntProperty) property).Value;
                            break;
                        case "hpTagBgcolor":
                            template.HpTagBgColor = ((WzIntProperty) property).Value;
                            break;
                        case "firstattack":
                        case "firstAttack":
                            template.FirstAttack = ((WzIntProperty) property).Value;
                            break;
                        case "explosiveReward":
                            template.ExplosiveReward = ((WzIntProperty) property).Value;
                            break;
                        case "link":
                            template.Link = ((WzStringProperty) property).Value;
                            break;
                        case "hidename":
                        case "hideName":
                            template.HideName = ((WzIntProperty) property).Value;
                            break;
                        case "removeAfter":
                            template.RemoveAfter = ((WzIntProperty) property).Value;
                            break;
                        case "noFlip":
                            template.NoFlip = ((WzIntProperty) property).Value;
                            break;
                        case "undead":
                            template.Undead = ((WzIntProperty) property).Value;
                            break;
                        case "damagedByMob":
                            template.DamagedByMob = ((WzIntProperty) property).Value;
                            break;
                        case "rareItemDropLevel":
                            template.RareItemDropLevel = ((WzIntProperty) property).Value;
                            break;
                        case "flySpeed":
                            template.FlySpeed = ((WzIntProperty) property).Value;
                            break;
                        case "publicReward":
                            template.PublicReward = ((WzIntProperty) property).Value;
                            break;
                        case "invincible":
                            template.Invincible = ((WzIntProperty) property).Value;
                            break;
                        case "upperMostLayer":
                            template.UpperMostLayer = ((WzIntProperty) property).Value;
                            break;
      
                        case "PartyReward":
                            template.PartyReward = ((WzStringProperty) property).Value;
                            break;
                        case "noregen":
                            template.NoRegen = ((WzIntProperty) property).Value;
                            break;
                        case "hideHP":
                            template.HideHP = ((WzIntProperty) property).Value;
                            break;
                        case "mbookID":
                            template.MBookID = ((WzIntProperty) property).Value;
                            break;
                        case "noDoom":
                            template.NoDoom = ((WzIntProperty) property).Value;
                            break;
                        case "fixedDamage":
                            template.FixedDamage = ((WzIntProperty) property).Value;
                            break;
                        case "removeQuest":
                            template.RemoveQuest = ((WzIntProperty) property).Value;
                            break;
                        case "ChargeCount":
                            template.ChargeCount = ((WzIntProperty) property).Value;
                            break;
                        case "AngerGauge":
                            template.AngerGauge = ((WzIntProperty) property).Value;
                            break;
                        case "chaseSpeed":
                            template.ChaseSpeed = ((WzIntProperty) property).Value;
                            break;
                        case "escort":
                            template.Escort = ((WzIntProperty) property).Value;
                            break;
                        case "removeOnMiss":
                            template.RemoveOnMiss = ((WzIntProperty) property).Value;
                            break;
                        case "coolDamage":
                            template.CoolDamage = ((WzIntProperty) property).Value;
                            break;
                        case "coolDamageProb":
                            template.CoolDamageProb = ((WzIntProperty) property).Value;
                            break;
                        case "0":
                            template._0 = ((WzIntProperty) property).Value;
                            break;
                        case "getCP":
                            template.GetCP = ((WzIntProperty) property).Value;
                            break;
                        case "cannotEvade":
                            template.CannotEvade = ((WzIntProperty) property).Value;
                            break;
                        case "dropItemPeriod":
                            template.DropItemPeriod = ((WzIntProperty) property).Value;
                            break;
                        case "onlyNormalAttack":
                            template.OnlyNormalAttack = ((WzIntProperty) property).Value;
                            break;
                        case "point":
                            template.Point = ((WzIntProperty) property).Value;
                            break;
                        case "fixDamage":
                            template.FixDamage = ((WzIntProperty) property).Value;
                            break;
                        case "weapon":
                            template.Weapon = ((WzIntProperty) property).Value;
                            break;
                        case "notAttack":
                            template.NotAttack = ((WzIntProperty) property).Value;
                            break;
                        case "doNotRemove":
                            template.DoNotRemove = ((WzIntProperty) property).Value;
                            break;
                        case "buff":
                            template.Buff = ((WzStringProperty) property).Value;
                            break;
                        case "Speed":
                            template.Speed = ((WzIntProperty) property).Value;
                            break;
                        case "cantPassByTeleport":
                            template.CantPassByTeleport = ((WzIntProperty) property).Value;
                            break;
                        case "phase":
                            template.Phase = ((WzIntProperty) property).Value;
                            break;
                        case "flyspeed":
                        case "FlySpeed":
                            template.FlySpeed = ((WzIntProperty) property).Value;
                            break;
                        case "dualGauge":
                            template.DualGauge = ((WzIntProperty) property).Value;
                            break;
                        case "disable":
                            template.Disable = ((WzIntProperty) property).Value;
                            break;
                        
                        case "ban":
                            template.MonsterBan = new TemplateMob.Ban();
                            foreach (WzImageProperty banProperty in property.WzProperties) {
                                switch (banProperty.Name) {
                                    case "0":
                                        foreach (WzImageProperty targetProperty in banProperty.WzProperties) {
                                            switch (targetProperty.Name) {
                                                case "field":
                                                    template.MonsterBan.TargetFieldID = ((WzIntProperty) targetProperty).Value;
                                                    break;
                                                case "portal":
                                                    template.MonsterBan.TargetPortalName = ((WzStringProperty) targetProperty).Value;
                                                    break;
                                                default:
                                                    Log.Info($"Unhandled Ban Target Property: {banProperty.Name} ({banProperty.GetType()})");
                                                    break;
                                                    
                                            }
                                        }
                                        break;
                                    case "banMsg":
                                        template.MonsterBan.Message = ((WzStringProperty) banProperty).Value;
                                        break;
                                    case "banMsgType":
                                        template.MonsterBan.MessageType = ((WzIntProperty) banProperty).Value;
                                        break;
                                    case "banType":
                                        template.MonsterBan.Type = ((WzIntProperty) banProperty).Value;
                                        break;
                                    default:
                                        Log.Info($"Unhandled Ban Message Property: {banProperty.Name} ({banProperty.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "loseItem":
                            template.LoseItems = new List<TemplateMob.LoseItem>();
                            foreach (WzImageProperty items in property.WzProperties) {
                                TemplateMob.LoseItem loseItem = new TemplateMob.LoseItem();
                                foreach (WzImageProperty loseProperty in items.WzProperties) {
                                    switch (loseProperty.Name) {
                                        case "id":
                                            loseItem.ID = ((WzIntProperty) loseProperty).Value;
                                            break;
                                        case "loseMsg":
                                            loseItem.Message = ((WzStringProperty) loseProperty).Value;
                                            break;
                                        case "loseMsgType":
                                            loseItem.MessageType = ((WzIntProperty) loseProperty).Value;
                                            break;
                                        case "notDrop":
                                            loseItem.Drop = ((WzIntProperty) loseProperty).Value == 1; 
                                            break;
                                        case "prop":
                                            loseItem.Prop = ((WzIntProperty) loseProperty).Value;
                                            break;
                                        case "x":
                                            loseItem.X = ((WzIntProperty) loseProperty).Value;
                                            break;
                                        default:
                                            Log.Info($"Unhandled Lose Item Property: {loseProperty.Name} ({loseProperty.GetType()})");
                                            break;
                                    }
                                }
                            }
                            break;
                        case "damagedBySelectedMob":
                            template.DamagedBySelectedMob = new List<int>();
                            foreach (WzImageProperty mob in property.WzProperties) {
                                template.DamagedBySelectedMob.Add(((WzIntProperty)mob).Value);
                            }
                            break;
                        case "damagedBySelectedSkill":
                            template.DamagedBySelectedSkill = new List<int>();
                            foreach (WzImageProperty skill in property.WzProperties) {
                                template.DamagedBySelectedSkill.Add(((WzIntProperty)skill).Value);
                            }
                            break;
                        case "healOnDestroy":
                            int? type = null, amount = null;
                            foreach (WzImageProperty heal in property.WzProperties) {
                                switch (heal.Name) {
                                    case "amount":
                                        amount = ((WzIntProperty) heal).Value;
                                        break;
                                    case "type":
                                        type = ((WzIntProperty) heal).Value;
                                        break;
                                    default:
                                        Log.Info($"Unhandled HealOnDestroy Property: {heal.Name} ({heal.GetType()})");
                                        break;
                                }
                            }

                            if (amount.HasValue && type.HasValue) {
                                template.HealOnDestroy = new Tuple<int, int>(type.Value, amount.Value);
                            }
                            break;
                        case "selfDestruction":
                            foreach (WzImageProperty destruction in property.WzProperties) {
                                switch (destruction.Name) {
                                    case "action":
                                        template.SelfDestruction = ((WzIntProperty) destruction).Value;
                                        break;
                                    default:
                                        Log.Info($"Unhandled SelfDestruction Property: {destruction.Name} ({destruction.GetType()})");
                                        break;
                                }
                            }
                            break;
                        case "skill":
                            template.Skills = new List<TemplateMob.Skill>();
                            foreach (WzImageProperty monsterSkillLevel in property.WzProperties) {
                                TemplateMob.Skill skill = new TemplateMob.Skill();
                                foreach (WzImageProperty monsterSkill in monsterSkillLevel.WzProperties) {
                                    switch (monsterSkill.Name) {
                                        case "action":
                                            skill.Action = ((WzIntProperty) monsterSkill).Value;
                                            break;
                                        case "effectAfter":
                                            skill.EffectAfter = ((WzIntProperty) monsterSkill).Value;
                                            break;
                                        case "level":
                                            skill.Level = ((WzIntProperty) monsterSkill).Value;
                                            break;
                                        case "skill":
                                            skill.ID = ((WzIntProperty) monsterSkill).Value;
                                            break;
                                        case "skillAfter":
                                            skill.NextID = ((WzIntProperty) monsterSkill).Value;
                                            break;
                                        default:
                                            Log.Info($"Unhandled MonsterSkill Property: {monsterSkill.Name} ({monsterSkill.GetType()})");
                                            break;
                                    }
                                }
                                template.Skills.Add(skill);
                            }
                            break;
                        case "revive":
                            template.Revives = new List<int>();
                            foreach (WzImageProperty revive in property.WzProperties) {
                                template.Revives.Add(((WzIntProperty)revive).Value);
                            }
                            break;
                        case "speak":
                            break;
                        case "default":
                            break;
                        default:
                            Log.Info($"Unhandled Mob Property: {property.Name}");
                            break;
                    }
                }
                
            }

        }

        public static void PrintDirectory() {
            List<string> uniqueProperties = new List<string>();
            foreach (WzImage image in WzProvider.Load("Mob").WzDirectory.WzImages) {
                foreach (WzImageProperty info in image.WzProperties.Where(Property => Property.Name == "info")) {
                    foreach (WzImageProperty property in info.WzProperties) {
                        if (uniqueProperties.All(name => name != property.Name)) {
                            uniqueProperties.Add(property.Name);
                            Log.Info($"{property.Name, 25} = {property.GetType()}, from ({image.Name})");
                        }
                    }
                }
            }
        }
    }
}