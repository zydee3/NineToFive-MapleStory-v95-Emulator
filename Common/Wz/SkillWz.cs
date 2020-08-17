using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.InteropServices;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class SkillWz {
        private const string WzName = "Skill";
        private static readonly ILog Log = LogManager.GetLogger(typeof(SkillWz));
        private static readonly Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();

        public static void SetSkill(Skill skill, ref List<WzImageProperty> skillProperties) {
            if (skill == null || skillProperties == null || skillProperties.Count == 0) return;

            foreach (WzImageProperty childProperty in skillProperties) {
                string propertyName = childProperty.Name;
                switch (propertyName) {
                    case "lt": {
                        WzVectorProperty vector = (WzVectorProperty) childProperty;
                        // skill.Lt = new Vector2(vector.GetPoint().X, vector.GetPoint().Y);
                        break;
                    }
                    case "rb": {
                        WzVectorProperty vector = (WzVectorProperty) childProperty;
                        // skill.Rb = new Vector2(vector.GetPoint().X, vector.GetPoint().Y);
                        break;
                    }
                    default: {
                        string propertyStringValue = "";
                        if (childProperty.GetType() == typeof(WzIntProperty)) {
                            propertyStringValue = ((WzIntProperty) childProperty).Value.ToString();
                        } else if (childProperty.GetType() == typeof(WzStringProperty)) {
                            propertyStringValue = ((WzStringProperty) childProperty).Value;
                        } else {
                            Log.Info($"Unhandled Property Type: {propertyName}({propertyName.GetType()})");
                        }

                        // switch (propertyName) {
                        //     case "acc":
                        //         skill.Acc = propertyStringValue;
                        //         break;
                        //     case "asrR":
                        //         skill.AsrR = propertyStringValue;
                        //         break;
                        //     case "attackCount":
                        //         skill.AttackCount = propertyStringValue;
                        //         break;
                        //     case "bulletCount":
                        //         skill.BulletCount = propertyStringValue;
                        //         break;
                        //     case "cooltime":
                        //         skill.Cooltime = propertyStringValue;
                        //         break;
                        //     case "cr":
                        //         skill.Cr = propertyStringValue;
                        //         break;
                        //     case "criticaldamageMax":
                        //         skill.CriticaldamageMax = propertyStringValue;
                        //         break;
                        //     case "criticaldamageMin":
                        //         skill.CriticaldamageMin = propertyStringValue;
                        //         break;
                        //     case "damage":
                        //         skill.Damage = propertyStringValue;
                        //         break;
                        //     case "damR":
                        //         skill.DamR = propertyStringValue;
                        //         break;
                        //     case "dot":
                        //         skill.Dot = propertyStringValue;
                        //         break;
                        //     case "dotInterval":
                        //         skill.DotInterval = propertyStringValue;
                        //         break;
                        //     case "dotTime":
                        //         skill.DotTime = propertyStringValue;
                        //         break;
                        //     case "emdd":
                        //         skill.Emdd = propertyStringValue;
                        //         break;
                        //     case "epad":
                        //         skill.Epad = propertyStringValue;
                        //         break;
                        //     case "epdd":
                        //         skill.Epdd = propertyStringValue;
                        //         break;
                        //     case "er":
                        //         skill.Er = propertyStringValue;
                        //         break;
                        //     case "eva":
                        //         skill.Eva = propertyStringValue;
                        //         break;
                        //     case "expR":
                        //         skill.ExpR = propertyStringValue;
                        //         break;
                        //     case "hpCon":
                        //         skill.HpCon = propertyStringValue;
                        //         break;
                        //     case "ignoreMobpdpR":
                        //         skill.IgnoreMobpdpR = propertyStringValue;
                        //         break;
                        //     case "jump":
                        //         skill.Jump = propertyStringValue;
                        //         break;
                        //     case "mad":
                        //         skill.Mad = propertyStringValue;
                        //         break;
                        //     case "mastery":
                        //         skill.Mastery = propertyStringValue;
                        //         break;
                        //     case "maxLevel":
                        //         skill.MaxLevel = propertyStringValue;
                        //         break;
                        //     case "mdd":
                        //         skill.Mdd = propertyStringValue;
                        //         break;
                        //     case "mhpR":
                        //         skill.MhpR = propertyStringValue;
                        //         break;
                        //     case "mobCount":
                        //         skill.MobCount = propertyStringValue;
                        //         break;
                        //     case "morph":
                        //         skill.Morph = propertyStringValue;
                        //         break;
                        //     case "mp":
                        //         skill.Mp = propertyStringValue;
                        //         break;
                        //     case "mpCon":
                        //         skill.MpCon = propertyStringValue;
                        //         break;
                        //     case "pad":
                        //         skill.Pad = propertyStringValue;
                        //         break;
                        //     case "padX":
                        //         skill.PadX = propertyStringValue;
                        //         break;
                        //     case "pdd":
                        //         skill.Pdd = propertyStringValue;
                        //         break;
                        //     case "pddR":
                        //         skill.PddR = propertyStringValue;
                        //         break;
                        //     case "prop":
                        //         skill.Prop = propertyStringValue;
                        //         break;
                        //     case "range":
                        //         skill.Range = propertyStringValue;
                        //         break;
                        //     case "speed":
                        //         skill.Speed = propertyStringValue;
                        //         break;
                        //     case "subProp":
                        //         skill.SubProp = propertyStringValue;
                        //         break;
                        //     case "subTime":
                        //         skill.SubTime = propertyStringValue;
                        //         break;
                        //     case "t":
                        //         skill.T = propertyStringValue;
                        //         break;
                        //     case "terR":
                        //         skill.TerR = propertyStringValue;
                        //         break;
                        //     case "time":
                        //         skill.Time = propertyStringValue;
                        //         break;
                        //     case "u":
                        //         skill.U = propertyStringValue;
                        //         break;
                        //     case "v":
                        //         skill.V = propertyStringValue;
                        //         break;
                        //     case "w":
                        //         skill.W = propertyStringValue;
                        //         break;
                        //     case "x":
                        //         skill.X = propertyStringValue;
                        //         break;
                        //     case "y":
                        //         skill.Y = propertyStringValue;
                        //         break;
                        //     case "z":
                        //         skill.Z = propertyStringValue;
                        //         break;
                        //     case "selfDestruction":
                        //         skill.SelfDestruction = propertyStringValue;
                        //         break;
                        //     case "itemCon":
                        //         skill.ItemCon = propertyStringValue;
                        //         break;
                        //     case "itemConNo":
                        //         skill.ItemCon = propertyStringValue;
                        //         break;
                        //     case "bulletConsume":
                        //         skill.BulletConsume = propertyStringValue;
                        //         break;
                        //     case "emmp":
                        //         skill.Emmp = propertyStringValue;
                        //         break;
                        //     case "emhp":
                        //         skill.Emhp = propertyStringValue;
                        //         break;
                        //     case "action":
                        //         skill.Action = propertyStringValue;
                        //         break;
                        //     case "mesoR":
                        //         skill.MesoR = propertyStringValue;
                        //         break;
                        //     case "madX":
                        //         skill.MadX = propertyStringValue;
                        //         break;
                        //     case "mmpR":
                        //         skill.MmpR = propertyStringValue;
                        //         break;
                        //     case "hp":
                        //         skill.Hp = propertyStringValue;
                        //         break;
                        //     case "moneyCon":
                        //         skill.MoneyCon = propertyStringValue;
                        //         break;
                        //     case "itemConsume":
                        //         skill.ItemConsume = propertyStringValue;
                        //         break;
                        //     case "mddR":
                        //         skill.MddR = propertyStringValue;
                        //         break;
                        //     default:
                        //         Log.Info($"Unhandled Skill Property: {propertyName}");
                        //         break;
                        // }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a dictionary of all skills found within a single job.
        /// </summary>
        /// <param name="jobID">Id of job from where skills are being retrieved.</param>
        /// <returns>Dictionary of found skills.</returns>
        /// <note>
        /// </note>
        public static Dictionary<int, Skill> GetFromJob(int jobID) {
            Dictionary<int, Skill> skillsRetrieved = new Dictionary<int, Skill>();
            WzFile _wz = WzProvider.Load(WzName);

            if (_wz != null) {
                foreach (WzImage job in _wz.WzDirectory.WzImages) {
                    if (job.Name != $"{jobID}.img") continue;

                    // Current Path: Skill.wz/{JobID}.img/skill
                    WzImageProperty skills = job.GetFromPath("skill");
                    if (skills == null) break;

                    // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}
                    foreach (WzImageProperty skill in skills.WzProperties) {
                        if (int.TryParse(skill.Name, out int SkillID)) {
                            // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}/common
                            WzImageProperty image = skill.GetFromPath("common");
                            if (image == null || image.WzProperties.Count == 0) continue;
                            List<WzImageProperty> skillProperties = skill.GetFromPath("common").WzProperties;

                            // Skill s = new Skill();
                            // SetSkill(s, ref skillProperties);
                            // skillsRetrieved.Add(SkillID, s);
                        }
                    }
                }
            }

            return skillsRetrieved;
        }

        /// <summary>
        /// Retrieves all skill ids where the predicate is evaluated as true.
        /// </summary>
        /// <param name="predicate">Condition to add to returned dictionary.</param>
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
        public static Dictionary<int, Skill> GetSkills() {
            WzFile wz = WzProvider.Load(WzName);

            foreach (WzImage job in wz.WzDirectory.WzImages) {
                var name = job.Name.Substring(0, job.Name.LastIndexOf(".", StringComparison.Ordinal));
                if (!int.TryParse(name, out var jobId)) continue;
                Console.WriteLine($"========== {jobId} ==========");
                foreach (var skill in job.GetFromPath("skill").WzProperties) {
                    var common = skill.GetFromPath("common");
                    var s = new Skill(int.Parse(skill.Name));
                    Console.Write($"Parsing {s.Id}...\t");
                    if (common != null) {
                        s.Common = new Dictionary<string, string>(common.WzProperties.Count);
                        foreach (var c in common.WzProperties) {
                            switch (c.Name) {
                                case "maxLevel":
                                    // (e.g. 13100004) uses WzStringProperty for some reason...
                                    s.MaxLevel = (common["maxLevel"] as WzIntProperty)?.Value ?? int.Parse((common["maxLevel"] as WzStringProperty)!.Value);
                                    break;
                                default:
                                    s.Common.Add(c.Name, c.WzValue.ToString());
                                    break;
                            }
                        }
                    }

                    s.MasterLevel = ((WzIntProperty) skill["masterLevel"])?.Value ?? 0;

                    Skills.Add(s.Id, s);
                    Console.WriteLine(s);
                }

                // Current Path: Skill.wz/{JobID}.img/skill
                // WzImageProperty skills = job.GetFromPath("skill");
                // if (skills == null) continue;
                //
                // // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}
                // foreach (WzImageProperty skill in skills.WzProperties) {
                //     if (!int.TryParse(skill.Name, out int skillId)) continue;
                //
                //     // Current Path: Skill.wz/{JobID}.img/skill/{SkillID}/common
                //     WzImageProperty image = skill.GetFromPath("common");
                //     if (image == null || image.WzProperties.Count == 0) continue;
                //     List<WzImageProperty> SkillProperties = skill.GetFromPath("common").WzProperties;
                //
                //     Skill s = new Skill();
                //     SetSkill(s, ref SkillProperties);
                //     Skills.Add(skillId, s);
                // }
            }

            return Skills;
        }
    }
}