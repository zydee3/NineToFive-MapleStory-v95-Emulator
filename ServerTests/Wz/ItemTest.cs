using NineToFive.Game.Storage;

namespace ServerTests.Wz {
    public class ItemTest {
        public static void Test() {
            Item Cash = new Item(5010008, true);
            Item Consume = new Item(2000011, true);
            Item Etc = new Item(04000014, true);
            Item Install = new Item(03010024, true);
            Item Pet = new Item(5000029, true);
        }
    }
}