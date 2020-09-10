using System;
using System.Numerics;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class SkillWz {
        private const string WzName = "Skill";
        private static readonly ILog Log = LogManager.GetLogger(typeof(SkillWz));

        public static int LoadSkills() {
            if (WzCache.Skills.Count > 0) {
                WzCache.Skills.Clear();
                GC.Collect();
            }

            WzFile wz = WzProvider.Load(WzName);

            var jobs = wz.WzDirectory.WzImages;
            foreach (var job in jobs) {
                var name = job.Name.Substring(0, job.Name.LastIndexOf(".", StringComparison.Ordinal));
                if (!int.TryParse(name, out _)) continue;
                foreach (var skill in job.GetFromPath("skill").WzProperties) {
                    ParseSkill(skill);
                }
            }

            return WzCache.Skills.Count;
        }

        private static void ParseSkill(WzImageProperty skillImg) {
            var s = new Skill(int.Parse(skillImg.Name)) {
                MasterLevel = ((WzIntProperty) skillImg["masterLevel"])?.Value ?? 0,
                Weapon = skillImg["weapon"]?.GetInt() ?? 0,
                SkillType = (byte) (skillImg["skillType"]?.GetInt() ?? 0),
            };

            var common = skillImg.GetFromPath("common");
            if (common != null) {
                s.MaxLevel = common["maxLevel"].GetInt();
                foreach (var c in common.WzProperties) {
                    ParseSkillProperty(c, s, c.WzValue.ToString());
                }
            } else {
                var levels = skillImg.GetFromPath("level");
                s.MaxLevel = levels.WzProperties.Count;
                foreach (var level in levels.WzProperties) {
                    foreach (var p in level.WzProperties) {
                        if (p is WzStringProperty || p is WzSubProperty) {
                            // Console.WriteLine($"Job: {jobId}, Skill {s.Id}, Property: {p.Name} : Can't parse; skipping...");
                            continue;
                        }

                        int nLevel = int.Parse(level.Name) - 1; // index starts at 0 :coolcat:
                        ParseSkillProperty(p, s, null, nLevel);
                    }
                }
            }

            if (s.SkillType == 2)
                s.CTS[SecondaryStat.Booster] = s.X;
            switch ((Skills) s.Id) {
                case Skills.FighterPowerGuard:
                case Skills.PagePowerGuard:
                    s.CTS[SecondaryStat.PowerGuard] = s.X;
                    break;
                case Skills.GameMasterHide:
                case Skills.SuperGameMasterHide:
                    s.X = new SkillValue(1, 1);
                    s.Time = new SkillValue(1, int.MaxValue);
                    goto case Skills.ThiefDarkSight;
                case Skills.ThiefDarkSight:
                case Skills.NightWalkerDarkSight:
                    s.CTS[SecondaryStat.DarkSight] = s.X;
                    break;
                case Skills.MarksmanMapleWarrior:
                case Skills.MechanicMapleWarrior:
                case Skills.AranMapleWarrior:
                case Skills.BishopMapleWarrior:
                case Skills.BuccaneerMapleWarrior:
                case Skills.CorsairMapleWarrior:
                case Skills.EvanMapleWarrior:
                case Skills.HeroMapleWarrior:
                case Skills.NightlordMapleWarrior:
                case Skills.PaladinMapleWarrior:
                case Skills.ShadowerMapleWarrior:
                case Skills.BattleMageMapleWarrior:
                case Skills.BladeMasterMapleWarrior:
                case Skills.DarkKnightMapleWarrior:
                case Skills.WildHunterMapleWarrior:
                case Skills.BowmasterMapleWarrior:
                case Skills.FirePoisonArchmageMapleWarrior:
                case Skills.IceLightningArchmageMapleWarrior:
                    s.CTS[SecondaryStat.BasicStatUp] = s.X;
                    break;
                case Skills.PirateDash:
                    s.CTS[SecondaryStat.DashSpeed] = s.X;
                    s.CTS[SecondaryStat.DashJump] = s.Y;
                    break;
                case Skills.BeginnerEchoOfHero:
                case Skills.LegendEchoOfHero:
                case Skills.NoblesseEchoOfHero:
                    s.CTS[SecondaryStat.PAD] = s.X;
                    s.CTS[SecondaryStat.MAD] = s.X;
                    break;
            }

            WzCache.Skills.Add(s.Id, s);
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
        private static void ParseSkillProperty(WzImageProperty property, Skill s, string expression, int skl = 0) {
            if (!Enum.TryParse(typeof(SecondaryStat), property.Name, true, out var stat) || stat == null) {
                switch (property.Name) {
                    default:
                        // Console.WriteLine($"Unhandled {s}:\r\n\t property {property}");
                        break;
                    case "maxLevel":
                        return;
                    case "mobCount":
                        ParseSkillLevel(property, s, s.MobCount, expression, skl);
                        break;
                    case "range":
                        ParseSkillLevel(property, s, s.Range, expression, skl);
                        break;
                    case "attackCount":
                        ParseSkillLevel(property, s, s.AttackCount, expression, skl);
                        break;
                    case "x":
                        ParseSkillLevel(property, s, s.X, expression, skl);
                        break;
                    case "y":
                        ParseSkillLevel(property, s, s.Y, expression, skl);
                        break;
                    case "damage":
                        ParseSkillLevel(property, s, s.Damage, expression, skl);
                        break;
                    case "time":
                        ParseSkillLevel(property, s, s.Time, expression, skl);
                        break;
                    case "cooltime":
                        ParseSkillLevel(property, s, s.CoolTime, expression, skl);
                        break;
                    case "mpCon":
                        ParseSkillLevel(property, s, s.MpCon, expression, skl);
                        break;
                    case "lt":
                        ParseSkillLevel(property, s, s.Lt, expression, skl);
                        break;
                    case "rb":
                        ParseSkillLevel(property, s, s.Rb, expression, skl);
                        break;
                }

                return;
            }

            var cts = (SecondaryStat) stat;
            s.CTS.TryGetValue(cts, out var v);
            if (v == null) {
                v = new SkillValue(s.MaxLevel);
                s.CTS.TryAdd(cts, v);
            }
            ParseSkillLevel(property, s, v, expression, skl);
        }

        private static void ParseSkillLevel(WzObject p, Skill s, SkillValue v, string expression, int skl) {
            if (expression != null) v.Eval(s, expression);
            if (p is WzVectorProperty vec) v[skl] = new Vector2(vec.X.Value, vec.Y.Value);
            else v[skl] = p.GetInt();
        }
    }
}