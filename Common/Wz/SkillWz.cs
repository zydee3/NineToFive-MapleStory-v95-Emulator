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
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class SkillWz {
        private const string WzName = "Skill";
        private static readonly ILog Log = LogManager.GetLogger(typeof(SkillWz));

        public static int LoadSkills() {
            WzFile wz = WzProvider.Load(WzName);
            var Skills = WzCache.Skills;
            
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

            return Skills.Count;
        }

        /// <summary>
        /// <para>
        /// If <paramref name="expression"/> is null then <paramref name="skl"/> is used
        /// to assign the value of a skill property at the specified level. Otherwise the expression is evaluated and applied to all available
        /// levels for the specified <see cref="s"/>.
        /// </para>
        /// </summary>
        /// <param name="property">skill property retrieved from the wz file</param>
        /// <param name="s">skill object the property belongs to</param>
        /// <param name="expression">math expression if the value can be scaled with the skill level</param>
        /// <param name="skl">skill level that is being parsed, if expression is not specified</param>
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