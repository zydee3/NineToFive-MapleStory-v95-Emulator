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
        ///     Parses skill properties data contained inside CommonImage
        /// </summary>
        /// <param name="Skill">Skill object being loaded</param>
        /// <param name="CommonImage">Image loaded from WzFile containing skill properties data.</param>
        /// <note>
        ///     I had to store all Values as a string because some Values were stored as both a WzIntProperty or WzStringProperty
        ///     depending on the skill so I wasn't able to store the string in an integer variable sometimes.
        /// </note>
        public static void SetSkill(Skill Skill, WzImageProperty CommonImage) {
            if (Skill == null || CommonImage == null) return;
            
            foreach (WzImageProperty ChildProperty in CommonImage.WzProperties) {
                string PropertyName = ChildProperty.Name;
                switch (PropertyName) {
                    case "lt": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        Skill.lt = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    case "rb": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        Skill.rb = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    default: {
                        if (SkillProperties.TryParse(PropertyName, out SkillProperties Property)) {
                            if (ChildProperty.GetType() == typeof(WzIntProperty)) {
                                Skill.Values[(int)Property] = ((WzIntProperty) ChildProperty).Value.ToString();
                            } else {
                                Skill.Values[(int)Property] = ((WzStringProperty) ChildProperty).Value;
                            }
                        } else {
                            Console.WriteLine($"Unhandled Skill Property: {PropertyName}");
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
                            WzImageProperty Common = Job.GetFromPath("common");
                            if (Common == null) continue;
                            
                            Skill S = new Skill();
                            SetSkill(S, Common);
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
                        WzImageProperty Common = Skill.GetFromPath("common");
                        if (Common == null) continue;
                        
                        Skill S = new Skill();
                        SetSkill(S, Common);
                        SkillsRetrieved.Add(SkillID, S);
                    }
                }
            }

            return SkillsRetrieved;
        }
    }
}