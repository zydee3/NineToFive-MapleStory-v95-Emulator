using System.Security.Cryptography.X509Certificates;
using NineToFive.Game.Storage;

namespace ServerTests.Wz {
    public class ItemTest {
        
        public static void Test() {
            Item Cash = new Item(5010008);
            Item Consume = new Item(2000011);
            Item Etc = new Item(04000014);
            Item Install = new Item(03010024);
            Item Pet = new Item(5000029);
        }
    }
}