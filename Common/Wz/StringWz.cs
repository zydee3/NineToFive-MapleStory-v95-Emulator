using System;
using System.Linq;
using MapleLib.WzLib;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class StringWz {
        private const string WzName = "String";
        
        public static int CacheMobDrops() {
            foreach(WzImage image in WzProvider.Load(WzName).WzDirectory.WzImages) {
                if (!image.Name.Equals("MonsterBook.img")) continue;

                foreach (WzImageProperty mob in image.WzProperties) {
                    WzImageProperty drops = mob?.GetFromPath("reward");
                    if (drops == null || !int.TryParse(mob.Name, out int mobId)) continue;

                    int index = 0;
                    int[] itemIds = new int[drops.WzProperties.Count];
                    foreach (WzImageProperty drop in drops.WzProperties) {
                        itemIds[index++] = drop.GetInt();
                    }

                    if (index == 0) {
                        Console.WriteLine($"No drops for {mobId}");
                    }

                    if (!WzCache.MobDrops.TryAdd(mobId, itemIds)) {
                        Console.WriteLine($"Duplicate: {mobId}");
                    }
                }
            }

            return WzCache.MobDrops.Count;
        }
    }
}