using System.Collections.Generic;
using MapleLib.WzLib;
using NineToFive.Game;

namespace NineToFive.Wz {
    public class SkillWz {
        private const string WzName = "Skill";

        public static Skill Get(int SkillID) {
            return new Skill(WzProvider.GetWzProperty(WzProvider.Load(WzName), $"{(SkillID / 10000)}.img/skill/{SkillID}/common"));
        }

        public static Dictionary<int, Skill> GetFromJob(int JobID) {
            Dictionary<int, Skill> SkillsRetrieved = new Dictionary<int, Skill>();
            WzFile _wz = WzProvider.Load("Skill");

            if (_wz != null) {
                foreach (WzImage Job in WzProvider.Load("Skill").WzDirectory.WzImages) {
                    if (Job.Name != $"{JobID}.img") continue;
                    WzImageProperty Skills = Job.GetFromPath("skill");

                    if (Skills == null) break;
                    foreach (WzImageProperty Skill in Skills.WzProperties) {
                        if (int.TryParse(Skill.Name, out int SkillID)) { // 
                            SkillsRetrieved.Add(SkillID, new Skill(Skill.GetFromPath("common")));
                        }
                    }
                }
            }

            return SkillsRetrieved;
        }
    }
}