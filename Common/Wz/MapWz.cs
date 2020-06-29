using System;
using MapleLib.WzLib;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class MapWz {
        private const string WzName = "Map";

        public static void GetField(int FieldID) {
            
        }

        public static TemplateField GetTemplateField(int FieldID) {
            string PathToMapImage = $"Map/Map{FieldID/100000000}/{FieldID}.img";
            WzImageProperty MapImage = WzProvider.GetWzProperty(WzProvider.Load("Map"), PathToMapImage);
            return new TemplateField(MapImage);
        }

        public static void SetField(Field Field, WzImageProperty MapImage) {
            if (Field == null || MapImage == null) return;

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