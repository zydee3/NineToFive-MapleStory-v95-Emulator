using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure.Data;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class MapWz {
        private const string WzName = "Map";
        
        /// <summary>
        /// Sets the the field variables of the field being passed in.
        /// </summary>
        /// <param name="Field">Field to be initialized.</param>
        /// <param name="MapImage">Image loaded from wz containing the field's data.</param>
        public static void SetField(Field Field, ref List<WzImageProperty> MapProperties) {
            if (Field == null || MapProperties == null) return;

            
            
            //Dictionary<uint, object> TemplateFields = Server.Worlds[0].Templates[(int) TemplateType.Field];
            //if (!TemplateFields.TryGetValue(Field.ID, out object Template)) {
                TemplateField Template = new TemplateField();
                SetTemplateField((TemplateField) Template, ref MapProperties);
                //TemplateFields.Add(Field.ID, Template);
            //}

            if (Template == null) return;
            Field.Properties = (TemplateField) Template;
        }

        public static void SetTemplateField(TemplateField Template, ref List<WzImageProperty> MapProperties) {
            if (Template == null || MapProperties == null || MapProperties.Count == 0) return;
            
            foreach (WzImageProperty Node in MapProperties) {
                //PrintDirectory(Node);
                switch (Node.Name) {
                    case "back": {}
                        break;
                    case "clock": {}
                        break;
                    case "foothold":
                        LoadFootholds(Node);
                        break;
                    case "info":
                        LoadInfo(Template, Node);
                        break;
                    case "ladderRope": {}
                        break;
                    case "life":
                        if (Template.LoadLife) LoadLife(Node);
                        break;
                    case "miniMap": {}
                        break;
                    case "portal": {}
                        break;
                    case "reactor": {}
                        break;
                    default:
                        if (!int.TryParse(Node.Name, out int _)) {
                            Console.WriteLine($"Unhandled Map Node: {Node.Name}");   
                        }
                        break;
                }
            }
        }

        public static void PrintDirectory(WzImageProperty Parent) {
            foreach(WzImageProperty InternalProperty in Parent.WzProperties) {
                Console.WriteLine($"-- {InternalProperty.Name}({InternalProperty.GetType()})");
                Console.WriteLine($"---- {InternalProperty.WzValue}");
                if (InternalProperty.Name == "fieldLimit") {
                    foreach (FieldLimitType Type in Enum.GetValues(typeof(FieldLimitType))) {
                        Console.WriteLine($"{Type} : {Type.Check((int)InternalProperty.WzValue)}");
                    }
                }
            }
        }
        
        public static void LoadFootholds(WzImageProperty FootholdsImage) {
            
            foreach (WzImageProperty Collection in FootholdsImage.WzProperties) {
                //Console.WriteLine(Collection.Name);
            }
        }

        public static void LoadLife(WzImageProperty LifeImage) {
            
        }

        /*
            version      =WzIntProperty
            cloud        =WzIntProperty
            town         =WzIntProperty
            returnMap    =WzIntProperty
            forcedReturn =WzIntProperty
            mobRate      =WzFloatProperty
            bgm          =WzStringProperty
            mapMark      =WzStringProperty
            hideMinimap  =WzIntProperty
            fieldLimit   =WzIntProperty
            VRTop        =WzIntProperty
            VRLeft       =WzIntProperty
            VRBottom     =WzIntProperty
            VRRight      =WzIntProperty
            swim         =WzIntProperty
         */
        public static void LoadInfo(TemplateField Template, WzImageProperty InfoImage) {
            foreach (WzImageProperty Property in InfoImage.WzProperties) {
                switch (Property.Name) {
                    case "bgm":
                        Template.BackgroundMusic = ((WzStringProperty) Property).Value;
                        break;
                    case "fieldLimit":
                        Template.FieldLimits = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
                        foreach (FieldLimitType Type in Enum.GetValues(typeof(FieldLimitType))) {
                            Template.FieldLimits[(int) Type] = Type.Check((int) Property.WzValue);
                        }
                        break;
                    case "forcedReturn":
                        Template.ForcedReturn = ((WzIntProperty) Property).Value;
                        break;
                    case "returnMap":
                        break;
                    case "mobRate":
                        Template.MobRate = ((WzFloatProperty) Property).Value;
                        break;
                    case "onFirstUserEnter":
                        Template.OnFirstUserEnter = ((WzStringProperty) Property).Value;
                        break;
                    case "onUserEnter":
                        Template.OnUserEnter = ((WzStringProperty) Property).Value;
                        break;
                    case "fly":
                        Template.Fly = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "swim":
                        Template.Swim = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "town":
                        Template.Town = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "cloud": 
                    case "hideMinimap": 
                    case "mapMark":
                    case "version":
                    case "VRBottom":
                    case "VRLeft":
                    case "VRRight":
                    case "VRTop":
                        break;
                    default:
                        Console.WriteLine($"Unhandled Map/Info Property: {Property.Name}");
                        break;
                }
            }
        }
        
    }
}