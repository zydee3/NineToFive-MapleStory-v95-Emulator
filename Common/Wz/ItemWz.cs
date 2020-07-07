using System.Collections.Generic;
using log4net;
using MapleLib.WzLib;
using NineToFive.Game.Storage;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public class ItemWz {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemWz));

        public static void SetItem(Item item) {
            if (item == null) return;

            if (!WzCache.Items.TryGetValue(item.Id, out Item template)) {
                string itemCategory = ItemConstants.GetItemCategory(item.Id);
                if (itemCategory == "" || itemCategory == "Special" || itemCategory == "ItemOption") return;

                string subItemCategory = (item.Id / 10000).ToString().PadLeft(4, '0');
                string pathToItemImage = $"{itemCategory}/{subItemCategory}.img/{item.Id.ToString().PadLeft(8, '0')}";

                List<WzImageProperty> itemProperties = WzProvider.GetWzProperties(WzProvider.Load("Item"), pathToItemImage);
                if (itemProperties == null) return;

                template = new Item(item.Id, false);
                foreach (WzImageProperty node in itemProperties) {
                    switch (node.Name) {
                        case "info":
                            foreach (WzImageProperty infoNode in node.WzProperties) {
                                switch (infoNode.Name) {
                                    case "icon":
                                    case "iconRaw":
                                        break;
                                    case "price":
                                        break;
                                    default:
                                        Log.Info($"Unhandled Info Node: {infoNode.Name}");
                                        break;
                                }
                            }

                            break;
                        case "spec":
                            foreach (WzImageProperty specNode in node.WzProperties) {
                                switch (specNode.Name) {
                                    case "moveTo":
                                        break;
                                    default:
                                        Log.Info($"Unhandled Spec Node: {specNode.Name}");
                                        break;
                                }
                            }

                            break;
                        case "effect":
                            foreach (WzImageProperty effectNode in node.WzProperties) {
                                switch (effectNode.Name) {
                                    default:
                                        Log.Info($"Unhandled Effect Node: {effectNode.Name}");
                                        break;
                                }
                            }

                            break;
                        default:
                            Log.Info($"Unhandled Item Property: {node.Name}");
                            break;
                    }
                }

                WzCache.Items.Add(item.Id, template);
            }
        }
    }
}