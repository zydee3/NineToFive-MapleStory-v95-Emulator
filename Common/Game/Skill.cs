using System;
using System.Numerics;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Constants;
using NineToFive.Wz;

namespace NineToFive.Game {
    public class Skill {
        public string[] Values { get; } = new string[Enum.GetNames(typeof(SkillProperties)).Length];
        public Vector2 lt { get; set; }
        public Vector2 rb { get; set; }

        /*
         * This constructor is meant for when multiple skills are being loaded, we should reuse the loaded WzFile.
         * @Param CommonImage<WzImageProperty>: Image of Node<common> which holds skill data. (Path: JobID.img/skill/SkillID/common.
         * @Return <Void>
        */
        public Skill(WzImageProperty CommonImage) {
            Load(CommonImage);
        }

        /*
         * This constructor is meant for when only a single skill is being loaded so the WzFile is being used as a singleton.
         * @Param CommonImage<WzImageProperty>: Image of Node<common> which holds skill data. (Path: JobID.img/skill/SkillID/common.
         * @Return <Void>
        */
        public Skill(int SkillID) {
            Load(WzProvider.GetWzProperty(WzProvider.Load("Skill"), $"{(SkillID / 10000)}.img/skill/{SkillID}/common"));
        }
        
        /*
         * Parses skill properties data contained inside CommonImage<WzImageProperty>
         * @Param CommonImage<WzImageProperty> Image loaded from WzFile containing skill properties data.
         * @Note I had to store all Values as a string because some Values were stored as both a WzIntProperty or WzStringProperty
         *       depending on the skill so I wasn't able to store the string in an integer variable sometimes.
         * @Return <Void>
        */
        private void Load(WzImageProperty CommonImage) {
            if (CommonImage == null) return;
            
            foreach (WzImageProperty ChildProperty in CommonImage.WzProperties) {
                string PropertyName = ChildProperty.Name;
                switch (PropertyName) {
                    case "lt": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        lt = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    case "rb": {
                        WzVectorProperty Vector = (WzVectorProperty) ChildProperty;
                        rb = new Vector2(Vector.GetPoint().X, Vector.GetPoint().Y);
                        break;
                    }
                    default: {
                        if (SkillProperties.TryParse(PropertyName, out SkillProperties Property)) {
                            if (ChildProperty.GetType() == typeof(WzIntProperty)) {
                                Values[(int)Property] = ((WzIntProperty) ChildProperty).Value.ToString();
                            } else {
                                Values[(int)Property] = ((WzStringProperty) ChildProperty).Value;
                            }
                        } else {
                            Console.WriteLine($"Unhandled Skill Property: {PropertyName}");
                        }
                        break;
                    }
                }
            }
        }
    }
}