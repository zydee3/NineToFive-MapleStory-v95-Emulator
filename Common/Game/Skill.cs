using System;
using System.Collections.Generic;
using System.Numerics;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Wz;

//todo handle two mage skills that still use levels

namespace NineToFive.Game {
    public class Skill {
        public string Acc { get; set; } 
        public string AsrR { get; set; }
        public string AttackCount { get; set; }
        public string BulletCount { get; set; }
        public string Cooltime { get; set; }
        public string Cr { get; set; }
        public string CriticaldamageMax { get; set; }
        public string CriticaldamageMin { get; set; }
        public string Damage { get; set; }
        public string DamR { get; set; }
        public string Dot { get; set; }
        public string DotInterval { get; set; }
        public string DotTime { get; set; }
        public string Emdd { get; set; }
        public string Epad { get; set; }
        public string Epdd { get; set; }
        public string Er { get; set; }
        public string Eva { get; set; }
        public string ExpR { get; set; }
        public string HpCon { get; set; }
        public string IgnoreMobpdpR { get; set; }
        public string Jump { get; set; }
        public string Mad { get; set; }
        public string Mastery { get; set; }
        public string MaxLevel { get; set; }
        public string Mdd { get; set; }
        public string MhpR { get; set; }
        public string MobCount { get; set; }
        public string Morph { get; set; }
        public string Mp { get; set; }
        public string MpCon { get; set; }
        public string Pad { get; set; }
        public string PadX { get; set; }
        public string Pdd { get; set; }
        public string PddR { get; set; }
        public string Prop { get; set; }
        public string Range { get; set; }
        public string Speed { get; set; }
        public string SubProp { get; set; }
        public string SubTime { get; set; }
        public string T { get; set; }
        public string TerR { get; set; }
        public string Time { get; set; }
        public string U { get; set; }
        public string V { get; set; }
        public string W { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string SelfDestruction { get; set; }
        public string ItemCon { get; set; }
        public string ItemConNo { get; set; }
        public string BulletConsume { get; set; }
        public string Emmp { get; set; }
        public string Emhp { get; set; }
        public string Action { get; set; }
        public string MesoR { get; set; }
        public string MadX { get; set; }
        public string MmpR { get; set; }
        public string Hp { get; set; }
        public string MoneyCon { get; set; }
        public string ItemConsume { get; set; }
        public string MddR { get; set; }
        public Vector2 Lt { get; set; }
        public Vector2 Rb { get; set; }
        
        /// <summary>
        ///     This constructor is meant for when multiple skills are being loaded, we should reuse the loaded WzFile.
        /// </summary>
        public Skill() { }

        /// <summary>
        ///     This constructor is meant for when only a single skill is being loaded so the WzFile is being used as a singleton.
        /// </summary>
        /// <param name="SkillID">Id of the skill being loaded.</param>
        public Skill(int SkillID) {
            List<WzImageProperty> SkillProperties = WzProvider.GetWzProperties(WzProvider.Load("Skill"), $"{(SkillID / 10000)}.img/skill/{SkillID}/common");
            SkillWz.SetSkill(this, ref SkillProperties);
        }
    }
}