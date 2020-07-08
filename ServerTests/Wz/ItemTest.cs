using NineToFive.Game.Storage;

namespace ServerTests.Wz {
    public static class ItemTest {
        public static void Test() {
            Item cash = new Item(5010008, true);
            Item consume = new Item(2000011, true);
            Item etc = new Item(04000014, true);
            Item install = new Item(03010024, true);
            Item pet = new Item(5000029, true);
        }
    }
}