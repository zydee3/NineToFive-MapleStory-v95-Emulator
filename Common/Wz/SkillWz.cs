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
        public static readonly Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();

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

        public static Dictionary<int, Skill> LoadSkills() {
            WzFile wz = WzProvider.Load(WzName);

            foreach (WzImage job in wz.WzDirectory.WzImages) {
                var name = job.Name.Substring(0, job.Name.LastIndexOf(".", StringComparison.Ordinal));
                if (!int.TryParse(name, out var jobId)) continue;
                foreach (var skill in job.GetFromPath("skill").WzProperties) {
                    var s = new Skill(int.Parse(skill.Name)) {
                        MasterLevel = ((WzIntProperty) skill["masterLevel"])?.Value ?? 0,
                        Weapon = skill["weapon"]?.GetInt() ?? 0,
                    };
                    
                    var common = skill.GetFromPath("common");
                    if (common != null) {
                        // (e.g. 13100004) uses WzStringProperty for some reason...
                        s.MaxLevel = common["maxLevel"].GetInt();
                        foreach (var c in common.WzProperties) {
                            SetSkillValue(c, s, c.WzValue.ToString());
                        }
                    } else {
                        var levels = skill.GetFromPath("level");
                        s.MaxLevel = levels.WzProperties.Count;
                        foreach (var level in levels.WzProperties) {
                            foreach (var p in level.WzProperties) {
                                if (p is WzStringProperty || p is WzSubProperty) {
                                    // Console.WriteLine($"Job: {jobId}, Skill {s.Id}, Property: {p.Name} : Can't parse; skipping...");
                                    continue;
                                }

                                int nLevel = int.Parse(level.Name) - 1; // index starts at 0 :coolcat:
                                SetSkillValue(p, s, null, nLevel);
                            }
                        }
                    }

                    Skills.Add(s.Id, s);
                }
            }

            return Skills;
        }

        /// <summary>
        /// <para>
        /// If <paramref name="expression"/> is null then <paramref name="skl"/> and <paramref name="value"/> is used
        /// to assign the value of a skill property. Otherwise the expression is evaluated and applied to all available
        /// levels for the specified skill.
        /// </para>
        /// </summary>
        /// <param name="property">wz property retrieved from the wz file</param>
        /// <param name="s">the skill object the property originates from</param>
        /// <param name="expression">math expression if the value can be scaled with the skill level</param>
        /// <param name="skl">skill level that is being parsed</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void SetSkillValue(WzImageProperty property, Skill s, string expression, int skl = 0) {
            if (!Enum.TryParse(typeof(TemporaryStat), property.Name, true, out var stat) || stat == null) {
                switch (property.Name) {
                    case "damage":
                        if (expression != null) s.Damage.Eval(s, property.WzValue.ToString());
                        else s.Damage[skl] = ((WzIntProperty) property).Value;

                        break;
                    case "time":
                        if (expression != null) s.Time.Eval(s, property.WzValue.ToString());
                        else s.Time[skl] = ((WzIntProperty) property).Value;

                        break;
                    case "cooltime":
                        if (expression != null) s.CoolTime.Eval(s, property.WzValue.ToString());
                        else s.CoolTime[skl] = ((WzIntProperty) property).Value;

                        break;
                    case "mpCon":
                        if (expression != null) s.MpCon.Eval(s, property.WzValue.ToString());
                        else s.MpCon[skl] = ((WzIntProperty) property).Value;
                        break;
                    case "lt": {
                        var v = ((WzVectorProperty) property);
                        s.Lt[skl] = new Vector2(v.X.Value, v.Y.Value);
                        break;
                    }
                    case "rb": {
                        var v = ((WzVectorProperty) property);
                        s.Rb[skl] = new Vector2(v.X.Value, v.Y.Value);
                        break;
                    }
                }

                return;
            }

            s.BitMask |= (TemporaryStat) stat;
            switch (stat) {
                case TemporaryStat.PAD:
                    if (expression == null) s.PAD[skl] = ((WzIntProperty) property).Value;
                    else s.PAD.Eval(s, expression);
                    break;
                case TemporaryStat.PDD:
                    if (expression == null) s.PDD[skl] = ((WzIntProperty) property).Value;
                    else s.PDD.Eval(s, expression);
                    break;
                case TemporaryStat.MAD:
                    if (expression == null) s.MAD[skl] = ((WzIntProperty) property).Value;
                    else s.MAD.Eval(s, expression);
                    break;
                case TemporaryStat.MDD:
                    if (expression == null) s.MDD[skl] = ((WzIntProperty) property).Value;
                    else s.MDD.Eval(s, expression);
                    break;
                case TemporaryStat.Acc:
                    if (expression == null) s.Acc[skl] = ((WzIntProperty) property).Value;
                    else s.Acc.Eval(s, expression);
                    break;
                case TemporaryStat.Eva:
                    if (expression == null) s.Eva[skl] = ((WzIntProperty) property).Value;
                    else s.Eva.Eval(s, expression);

                    break;
                case TemporaryStat.Hands:
                    break;
                case TemporaryStat.Speed:
                    if (expression == null) s.Speed[skl] = ((WzIntProperty) property).Value;
                    else s.Speed.Eval(s, expression);

                    break;
                case TemporaryStat.Jump:
                    if (expression == null) s.Jump[skl] = ((WzIntProperty) property).Value;
                    else s.Jump.Eval(s, expression);

                    break;
                case TemporaryStat.Ghost:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }
    }
}