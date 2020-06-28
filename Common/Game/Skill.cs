using System;
using System.Numerics;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;

namespace NineToFive.Game {
    public class Skill { 
        // Integer Properties
        public int maxLevel { get; set; }
        public int attackCount { get; set; }
        public int mad { get; set; }
        public int z { get; set; }
        public int bulletCount { get; set; }
        public int dotInterval { get; set; }
        public int pddR { get; set; }
        public int mhpR { get; set; }
        public int mmpR { get; set; }
        public int expR { get; set; }
        public int morph { get; set; }
        
        // String Properties
        public string range { get; set; }
        public string mobCount { get; set; }
        public string mp { get; set; }
        public string mpCon { get; set; }
        public string damR { get; set; }
        public string x { get; set; }
        public string time { get; set; }
        public string damage { get; set; }
        public string prop { get; set; }
        public string y { get; set; }
        public string v { get; set; }
        public string cooltime { get; set; }
        public string cr { get; set; }
        public string criticaldamageMin { get; set; }
        public string terR { get; set; }
        public string asrR { get; set; }
        public string w { get; set; }
        public string subProp { get; set; }
        public string pad { get; set; }
        public string mastery { get; set; }
        public string subTime { get; set; }
        public string u { get; set; }
        public string criticaldamageMax { get; set; }
        public string er { get; set; }
        public string ignoreMobpdpR { get; set; }
        public string epad { get; set; }
        public string epdd { get; set; }
        public string emdd { get; set; }
        public string speed { get; set; }
        public string jump { get; set; }
        public string eva { get; set; }
        public string acc { get; set; }
        public string t { get; set; }
        public string dot { get; set; }
        public string dotTime { get; set; }
        public string pdd { get; set; }
        public string mdd { get; set; }
        public string padX { get; set; }
        
        // Vector2 Properties
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
         * This constructor is meant for when only a single skill is being loaded.
         * @Param SkillID<int>: Target skill to be loaded.
         * @Return <Void>
        */
        //public Skill(int SkillID) {
        //    Load(provider.Wz.GetWzProperty("Skill.Wz", $"{(SkillID / 10000)}.img/skill/{SkillID}/common"));
        //}

        /*
         * Parses skill properties data contained inside CommonImage<WzImageProperty>
         * @Param CommonImage<WzImageProperty> Image loaded from WzFile containing skill properties data.
         * @Return <Void>
        */
        private void Load(WzImageProperty CommonImage) {
            if (CommonImage == null) return;
            
            foreach (WzImageProperty ChildProperty in CommonImage.WzProperties) {
                switch (ChildProperty.Name) {
                    
                    // Integer properties
                    case "maxLevel":
                        maxLevel = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "attackCount":
                        attackCount = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "mad":
                        mad = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "z":
                        z = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "bulletCount":
                        bulletCount = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "dotInterval":
                        dotInterval = ((WzIntProperty)ChildProperty).Value;
                        break;
                    case "pddR":  
                        pddR  = ((WzIntProperty)ChildProperty).Value; 
                        break;
                    case "mhpR":  
                        mhpR  = ((WzIntProperty)ChildProperty).Value; 
                        break;
                    case "mmpR":  
                        mmpR  = ((WzIntProperty)ChildProperty).Value; 
                        break;
                    case "expR":  
                        expR  = ((WzIntProperty)ChildProperty).Value; 
                        break;
                    case "morph": 
                        morph = ((WzIntProperty)ChildProperty).Value; 
                        break;
                      
                    
                    // String properties
                    case "range":
                        range = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "mobCount":
                        mobCount = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "mp":    
                        mp = ((WzStringProperty)ChildProperty).Value; 
                        break;
                    case "mpCon": 
                        mpCon = ((WzStringProperty)ChildProperty).Value; 
                        break;
                    case "damR": 
                        damR  = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "x":
                        x = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "time":
                        time = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "damage":
                        damage = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "prop":
                        prop = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "y":
                        y = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "v":
                        v = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "cooltime":
                        cooltime = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "cr":
                        cr = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "criticaldamageMin":
                        criticaldamageMin = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "terR":
                        terR = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "asrR":
                        asrR = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "w":
                        w = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "subProp":
                        subProp = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "pad":
                        pad = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "mastery":
                        mastery = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "subTime":
                        subTime = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "u":
                        u = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "criticaldamageMax":
                        criticaldamageMax = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "er":
                        er = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "ignoreMobpdpR":
                        ignoreMobpdpR = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "epad":
                        epad = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "epdd":
                        epdd = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "emdd":
                        emdd = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "speed":
                        speed = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "jump":
                        jump = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "eva":
                        eva = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "acc":
                        acc = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "t":
                        t = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "dot":
                        dot = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "dotTime":
                        dotTime = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "pdd":
                        pdd = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "mdd":
                        mdd = ((WzStringProperty)ChildProperty).Value;
                        break;
                    case "padX":
                        padX = ((WzStringProperty)ChildProperty).Value;
                        break;
                        
                    // Vector2 Properties
                    case "lt":
                        WzVectorProperty LTVector = (WzVectorProperty) ChildProperty;
                        lt = new Vector2(LTVector.GetPoint().X, LTVector.GetPoint().Y);
                        break;
                    case "rb":
                        WzVectorProperty RBVector = (WzVectorProperty) ChildProperty;
                        lt = new Vector2(RBVector.GetPoint().X, RBVector.GetPoint().Y);
                        break;
                        
                    default:
                        Console.WriteLine($"Unhandled Skill Property: {ChildProperty.Name}");
                        break;
                }
            }
        }
    }
}