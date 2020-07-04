using System;
using System.Collections.Generic;
using System.Numerics;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game;

namespace NineToFive.Wz {
    public static class SkillWz {
        private const string WzName = "Skill";

        /// <summary>
        ///     Parses skill properties data contained inside skill properties list.
        /// </summary>
        /// <param name="Skill">Skill object being loaded</param>
        /// <param name="CommonImage">Image loaded from WzFile containing skill properties data.</param>
        /// <note>
        ///     I had to store all Values as a string because some Values were stored as both a WzIntProperty or WzStringProperty
        ///     depending on the skill so I wasn't able to store the string in an integer variable sometimes.
        /// </note>
        public static void SetSkill(Skill Skill, ref List<WzImageProperty> SkillProperties) {
            if (Skill == null || SkillProperties == null || SkillProperties.Count == 0) return;
            
            foreach (WzImageProperty ChildProperty in SkillProperties) {
                string PropertyName = ChildProperty.Name;
                switch (PropertyName) {
                    case "lt": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        Skill.Lt = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    case "rb": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        Skill.Rb = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    default: {
                        string PropertyStringValue = "";
                        if (ChildProperty.GetType() == typeof(WzIntProperty)) {
                            PropertyStringValue = ((WzIntProperty) ChildProperty).Value.ToString();
                        } else if (ChildProperty.GetType() == typeof(WzStringProperty)) {
                            PropertyStringValue = ((WzStringProperty) ChildProperty).Value;
                        } else {
                            Console.WriteLine($"Unhandled Property Type: {PropertyName}({PropertyName.GetType()})");
                        }

                        switch (PropertyName) {
                            case "acc":
                                Skill.Acc = PropertyStringValue;
                                break;
                            case "asrR": 
                                Skill.AsrR = PropertyStringValue;
                                break;
                            case "attackCount": 
                                Skill.AttackCount = PropertyStringValue;
                                break;
                            case "bulletCount": 
                                Skill.BulletCount = PropertyStringValue;
                                break;
                            case "cooltime": 
                                Skill.Cooltime = PropertyStringValue;
                                break;
                            case "cr": 
                                Skill.Cr = PropertyStringValue;
                                break;
                            case "criticaldamageMax": 
                                Skill.CriticaldamageMax = PropertyStringValue;
                                break;
                            case "criticaldamageMin": 
                                Skill.CriticaldamageMin = PropertyStringValue;
                                break;
                            case "damage": 
                                Skill.Damage = PropertyStringValue;
                                break;
                            case "damR":
                                Skill.DamR = PropertyStringValue;
                                break;
                            case "dot": 
                                Skill.Dot = PropertyStringValue;
                                break;
                            case "dotInterval": 
                                Skill.DotInterval = PropertyStringValue;
                                break;
                            case "dotTime": 
                                Skill.DotTime = PropertyStringValue;
                                break;
                            case "emdd": 
                                Skill.Emdd = PropertyStringValue;
                                break;
                            case "epad": 
                                Skill.Epad = PropertyStringValue;
                                break;
                            case "epdd": 
                                Skill.Epdd = PropertyStringValue;
                                break;
                            case "er": 
                                Skill.Er = PropertyStringValue;
                                break;
                            case "eva": 
                                Skill.Eva = PropertyStringValue;
                                break;
                            case "expR": 
                                Skill.ExpR = PropertyStringValue;
                                break;
                            case "hpCon": 
                                Skill.HpCon = PropertyStringValue;
                                break;
                            case "ignoreMobpdpR": 
                                Skill.IgnoreMobpdpR = PropertyStringValue;
                                break;
                            case "jump": 
                                Skill.Jump = PropertyStringValue;
                                break;
                            case "mad": 
                                Skill.Mad = PropertyStringValue;
                                break;
                            case "mastery": 
                                Skill.Mastery = PropertyStringValue;
                                break;
                            case "maxLevel": 
                                Skill.MaxLevel = PropertyStringValue;
                                break;
                            case "mdd": 
                                Skill.Mdd = PropertyStringValue;
                                break;
                            case "mhpR": 
                                Skill.MhpR = PropertyStringValue;
                                break;
                            case "mobCount": 
                                Skill.MobCount = PropertyStringValue;
                                break;
                            case "morph": 
                                Skill.Morph = PropertyStringValue;
                                break;
                            case "mp": 
                                Skill.Mp = PropertyStringValue;
                                break;
                            case "mpCon": 
                                Skill.MpCon = PropertyStringValue;
                                break;
                            case "pad": 
                                Skill.Pad = PropertyStringValue;
                                break;
                            case "padX": 
                                Skill.PadX = PropertyStringValue;
                                break;
                            case "pdd": 
                                Skill.Pdd = PropertyStringValue;
                                break;
                            case "pddR": 
                                Skill.PddR = PropertyStringValue;
                                break;
                            case "prop": 
                                Skill.Prop = PropertyStringValue;
                                break;
                            case "range": 
                                Skill.Range = PropertyStringValue;
                                break;
                            case "speed": 
                                Skill.Speed = PropertyStringValue;
                                break;
                            case "subProp": 
                                Skill.SubProp = PropertyStringValue;
                                break;
                            case "subTime": 
                                Skill.SubTime = PropertyStringValue;
                                break;
                            case "t": 
                                Skill.T = PropertyStringValue;
                                break;
                            case "terR": 
                                Skill.TerR = PropertyStringValue;
                                break;
                            case "time": 
                                Skill.Time = PropertyStringValue;
                                break;
                            case "u": 
                                Skill.U = PropertyStringValue;
                                break;
                            case "v": 
                                Skill.V = PropertyStringValue;
                                break;
                            case "w": 
                                Skill.W = PropertyStringValue;
                                break;
                            case "x": 
                                Skill.X = PropertyStringValue;
                                break;
                            case "y": 
                                Skill.Y = PropertyStringValue;
                                break;
                            case "z":
                                Skill.Z = PropertyStringValue;
                                break;
                            case "selfDestruction": 
                                Skill.SelfDestruction = PropertyStringValue;
                                break;
                            case "itemCon":
                                Skill.ItemCon = PropertyStringValue;
                                break;
                            case "itemConNo": 
                                Skill.ItemCon = PropertyStringValue;
                                break;
                            case "bulletConsume": 
                                Skill.BulletConsume = PropertyStringValue;
                                break;
                            case "emmp": 
                                Skill.Emmp = PropertyStringValue;
                                break;
                            case "emhp": 
                                Skill.Emhp = PropertyStringValue;
                                break;
                            case "action": 
                                Skill.Action = PropertyStringValue;
                                break;
                            case "mesoR": 
                                Skill.MesoR = PropertyStringValue;
                                break;
                            case "madX": 
                                Skill.MadX = PropertyStringValue;
                                break;
                            case "mmpR": 
                                Skill.MmpR = PropertyStringValue;
                                break;
                            case "hp": 
                                Skill.Hp = PropertyStringValue;
                                break;
                            case "moneyCon": 
                                Skill.MoneyCon = PropertyStringValue;
                                break;
                            case "itemConsume": 
                                Skill.ItemConsume = PropertyStringValue;
                                break;
                            case "mddR": 
                                Skill.MddR = PropertyStringValue;
                                break;
                            default:
                                Console.WriteLine($"Unhandled Skill Property: {PropertyName}");
                                break;
                        }
                        break;
                    }
                }
            }
        }
                
        /// <summary>
        /// Returns a dictionary of all skills found within a single job.
        /// </summary>
        /// <param name="JobID">Id of job from where skills are being retrieved.</param>
        /// <returns>Dictionary of found skills.</returns>
        /// <note>
        /// </note>
        public static Dictionary<int, Skill> GetFromJob(int JobID) {
            Dictionary<int, Skill> SkillsRetrieved = new Dictionary<int, Skill>();
            WzFile _wz = WzProvider.Load(WzName);

            if (_wz != null) {
                foreach (WzImage Job in _wz.WzDirectory.WzImages) {
                    if (Job.Name != $"{JobID}.img") continue;
                    
                    // Current Path: Skill.wz/{JobID}.img/skill
                    WzImageProperty Skills = Job.GetFromPath("skill");
                    if (Skills == null) break;
                    
                    // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}
                    foreach (WzImageProperty Skill in Skills.WzProperties) {
                        if (int.TryParse(Skill.Name, out int SkillID)) { 
                            
                            // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}/common
                            List<WzImageProperty> SkillProperties = Job.GetFromPath("common").WzProperties;
                            if (SkillProperties == null || SkillProperties.Count == 0) continue;
                            
                            Skill S = new Skill();
                            SetSkill(S, ref SkillProperties);
                            SkillsRetrieved.Add(SkillID, S);
                        }
                    }
                }
            }

            return SkillsRetrieved;
        }
        
        /// <summary>
        /// Retrieves all skill ids where the predicate is evaluated as true.
        /// </summary>
        /// <param name="Predicate">Condition to add to returned dictionary.</param>
        /// <returns>Dictionary of all skills that match the predicate.</returns>
        /// <note>
        ///     The downside to this function is it iterates through the whole skill wz;
        ///     the operation is still very fast. If you just need skills from one job,
        ///     you should use SkillWz.GetFromJob(int JobID). Honestly, I probably
        ///     wouldn't even use it, but it's here if needed.
        /// </note>
        /// <note>
        ///    Average benchmark time: 00:00:01.3542697
        /// </note>
        public static Dictionary<int, Skill> GetSkills(Func<int, bool> Predicate) {
            Dictionary<int, Skill> SkillsRetrieved = new Dictionary<int, Skill>();
            WzFile _wz = WzProvider.Load(WzName);

            if (_wz != null) {
                foreach (WzImage Job in _wz.WzDirectory.WzImages) {
                    // Current Path: Skill.wz/{JobID}.img/skill
                    WzImageProperty Skills = Job.GetFromPath("skill");
                    if (Skills == null) continue;
                    
                    // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}
                    foreach (WzImageProperty Skill in Skills.WzProperties) {
                        if (!int.TryParse(Skill.Name, out int SkillID) || !Predicate(SkillID)) continue;
                        
                        // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}/common
                        List<WzImageProperty> SkillProperties = Skill.GetFromPath("common").WzProperties;
                        if (SkillProperties == null) continue;
                        
                        Skill S = new Skill();
                        SetSkill(S, ref SkillProperties);
                        SkillsRetrieved.Add(SkillID, S);
                    }
                }
            }

            return SkillsRetrieved;
        }
    }
}