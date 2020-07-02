using System;
using System.Collections.Generic;
using System.Linq;
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
            
            Dictionary<uint, object> TemplateFields = Server.Worlds[0].Templates[(int) TemplateType.Field];
            if (!TemplateFields.TryGetValue(Field.ID, out object Template)) {
                Template = new TemplateField();
                SetTemplateField((TemplateField) Template, ref MapProperties);
                TemplateFields.Add(Field.ID, Template);
            }

            if (Template == null) return;
            Field.Properties = (TemplateField)((TemplateField) Template).Clone(); // this is so stupid
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
                        LoadFootholds(Template, Node, true);
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
                            Console.WriteLine($"Unhandled Map Node: {Node.Name, 10}({Node.GetType()})");   
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
        
        public static void LoadFootholds(TemplateField Template, WzImageProperty FootholdsImage, bool ReduceFootholds) {
            Dictionary<uint, Foothold> Footholds = new Dictionary<uint, Foothold>();
            
            foreach (WzImageProperty Collection in FootholdsImage.WzProperties) {
                foreach (WzImageProperty Parent in Collection.WzProperties) {
                    foreach (WzImageProperty Child in Parent.WzProperties) {
                        if (!int.TryParse(Child.Name, out int ChildID)) continue;
                        Foothold Foothold = new Foothold();
                        
                        foreach (WzImageProperty Property in Child.WzProperties) {
                            switch (Property.Name) {
                                case "next":
                                    Foothold.Next = ((WzIntProperty) Property).Value;
                                    break;
                                case "prev": 
                                    Foothold.Prev = ((WzIntProperty) Property).Value;;
                                    break;
                                case "x1":
                                    Foothold.X1 = ((WzIntProperty) Property).Value;;
                                    break;
                                case "x2":
                                    Foothold.X2 = ((WzIntProperty) Property).Value;
                                    break;
                                case "y1":
                                    Foothold.Y1 = ((WzIntProperty) Property).Value;
                                    break;
                                case "y2":
                                    Foothold.Y2 = ((WzIntProperty) Property).Value;
                                    break;
                                case "piece":
                                    break;
                                default:
                                    Console.WriteLine($"Unhandled Foothold Property: {Property.Name, 10}({Property.PropertyType})");
                                    break;
                            }
                        }
                        Foothold.SetEndPoints();
                        Footholds.Add((uint) ChildID, Foothold);
                    } 
                }
            }

            if (!ReduceFootholds) {
                Template.Footholds = Footholds.Select(Entry => Entry.Value).ToArray();
                return;
            }

            //Console.WriteLine($"Initial Number of Footholds: {Footholds.Count}");
            
            IEnumerable<KeyValuePair<uint, Foothold>> FilteredHorizontal = Footholds.Where(Entry => Entry.Value.Y1 == Entry.Value.Y2).OrderBy(Entry => Entry.Value.LeftEndPoint.Item1);
            Dictionary<float, Tuple<uint, uint>> Pairs = new Dictionary<float, Tuple<uint, uint>>();
            List<uint> ToExclude = new List<uint>();
            
            foreach ((uint ID, Foothold Foothold) in FilteredHorizontal) {
                if (Pairs.TryGetValue(Foothold.Y1, out Tuple<uint, uint> Pair)) {
                    if (Foothold.LeftEndPoint.Item1 < Footholds[Pair.Item1].LeftEndPoint.Item1) {
                        Pairs[Foothold.Y1] = new Tuple<uint, uint>(ID, Pair.Item2);
                        ToExclude.Add(Pair.Item1);
                    } else if (Foothold.RightEndPoint.Item1 > Footholds[Pair.Item2].RightEndPoint.Item1) {
                        Pairs[Foothold.Y1] = new Tuple<uint, uint>(Pair.Item1, ID);
                        ToExclude.Add(Pair.Item2);
                    } else {
                        ToExclude.Add(ID);
                    }
                } else {
                    Pairs.Add(Foothold.Y1, new Tuple<uint, uint>(ID, ID));
                }
                
                //Console.WriteLine($"ID: {ID,5}, Left Point: {Foothold.LeftEndPoint.Item1,5}, X1: {Foothold.X1,5}, X2: {Foothold.X2,5}, Y1: {Foothold.Y1,5}, Y2: {Foothold.Y2,5}, Next: {Foothold.Next,5}, Prev: {Foothold.Prev,5}");
            }

            //todo: finish this slope reduction set
            /*
            foreach ((uint ID, Foothold Foothold) in Footholds) {
                if (ToExclude.Contains(ID) || Foothold.SlopeForm.Slope == 0) continue;
                    
                if (Pairs.TryGetValue(Foothold.SlopeForm.Slope, out Tuple<uint, uint> Pair)) {
                    Foothold Next = null, Prev = null;
                    if (Footholds.ContainsKey((uint)Foothold.Next)) {
                        Next = Footholds[(uint) Foothold.Next];
                    }

                    if (Footholds.ContainsKey((uint) Foothold.Prev)) {
                        Prev = Footholds[(uint) Foothold.Prev];
                    }

                    if (Next != null) {
                        
                    } else if (Prev != null) {
                        
                    } else {
                        
                    }
                } else {
                    Pairs.Add(Foothold.Y1, new Tuple<uint, uint>(ID, ID));
                }
            }
            */
            
            foreach ((float Y1, (uint Left, uint Right)) in Pairs) {
                Foothold Lf = Footholds[Left];
                (int X, int Y) = Lf.RightEndPoint = Footholds[Right].RightEndPoint;
                if (Lf.X1 == Lf.LeftEndPoint.Item1) {
                    Lf.X2 = X;
                    Lf.Y2 = Y;
                } else {
                    Lf.X1 = X;
                    Lf.Y1 = Y;
                }
                ToExclude.Add(Right);
                //Console.WriteLine($"Y1: {Y1, 10}, Footholds({Left}, {Right}), {"", 5}Pair({Footholds[Left].LeftEndPoint, 17}, {Footholds[Left].RightEndPoint})");
            }
            
            //Console.WriteLine($"Final Number of Footholds: {Footholds.Count - ToExclude.Count}");
            Template.Footholds = Footholds.Where(Entry => !ToExclude.Contains(Entry.Key)).Select(Entry => Entry.Value).ToArray();
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
                        Console.WriteLine($"Unhandled Map/Info Property: {Property.Name, 10}({Property.GetType()})");
                        break;
                }
            }
        }
        
    }
}