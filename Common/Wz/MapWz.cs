using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class MapWz {
        private const string WzName = "Map";
        
        public static void SetField(Field Field, WzImageProperty MapImage) {
            if (Field == null || MapImage == null) return;
            
            Dictionary<uint, object> TemplateFields = Server.Worlds[0].Templates[(int) TemplateType.Field];
            if (TemplateFields.TryGetValue(Field.ID, out object Template)) {
                Template = new TemplateField();
                SetTemplateField((TemplateField) Template, MapImage);
                TemplateFields.Add(Field.ID, Template);
            }

            if (Template == null) return;
            Field.Properties = (TemplateField) Template;
        }

        public static void SetTemplateField(TemplateField Template, WzImageProperty MapImage) {
            if (Template == null || MapImage == null) return;

            foreach (WzImageProperty Node in MapImage.WzProperties) {
                switch (Node.Name) {
                    case "back":
                        break;
                    case "clock":
                        break;
                    case "foothold":
                        break;
                    case "info":
                        break;
                    case "ladderRope":
                        break;
                    case "life":
                        break;
                    case "miniMap":
                        break;
                    case "portal":
                        break;
                    case "reactor":
                        break;
                    default:
                        Console.WriteLine($"Unhandled Map Node: {Node.Name}");
                        break;
                }
            }
        }
    }
}