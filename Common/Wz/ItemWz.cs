using System;
using System.Collections.Generic;
using MapleLib.WzLib;
using NineToFive.Constants;
using NineToFive.Game.Storage.Meta;
using Item = NineToFive.Game.Storage.Item;

namespace NineToFive.Wz {
    public class ItemWz {

        public static void SetItem(Item item, int itemId) {
            if (item == null) return;

            Dictionary<int, Item> items = Server.Worlds[0].Items;
            if (!items.ContainsKey(itemId)) {
                string itemCategory = Constants.Item.GetItemCategory(itemId);
                if (itemCategory == "" || itemCategory == "Special" || itemCategory == "ItemOption") return;
                
                string subItemCategory = (itemId / 10000).ToString().PadLeft(4, '0');
                string pathToItemImage = $"{itemCategory}/{subItemCategory}.img/{itemId.ToString().PadLeft(8, '0')}";
                
                List<WzImageProperty> itemProperties = WzProvider.GetWzProperties(WzProvider.Load("Item"), pathToItemImage);
                if (itemProperties == null) return;

                foreach (WzImageProperty node in itemProperties) {
                    switch (node.Name) {
                        case "info":
                            break;
                        case "spec":
                            break;
                        case "effect":
                            break;
                        default:
                            Console.WriteLine($"Unhandled Item Property: {node.Name}");
                            break;
                    }
                }
            } else {
                
            }
            
        }

    }
}