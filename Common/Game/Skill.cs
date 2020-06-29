using System;
using System.Numerics;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Wz;

//todo handle two mage skills that still use levels

namespace NineToFive.Game {
    public class Skill {
        public string[] Values { get; } = new string[Enum.GetNames(typeof(SkillProperties)).Length];
        public Vector2 lt { get; set; }
        public Vector2 rb { get; set; }
        
        /// <summary>
        ///     This constructor is meant for when multiple skills are being loaded, we should reuse the loaded WzFile.
        /// </summary>
        public Skill() { }

        /// <summary>
        ///     This constructor is meant for when only a single skill is being loaded so the WzFile is being used as a singleton.
        /// </summary>
        /// <param name="SkillID">Id of the skill being loaded.</param>
        public Skill(int SkillID) {
            SkillWz.SetSkill(this, WzProvider.GetWzProperty(WzProvider.Load("Skill"), $"{(SkillID / 10000)}.img/skill/{SkillID}/common"));
        }
    }
}