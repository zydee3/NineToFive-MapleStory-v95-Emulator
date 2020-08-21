using NineToFive;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor

            WzReaderTest.TestSkill();
            // WzReaderTest.TestMob();
            // WzReaderTest.TestField();
            // WzReaderTest.TestItem();
        }
    }
}