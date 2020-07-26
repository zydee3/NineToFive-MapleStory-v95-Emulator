using NineToFive;

namespace ServerTests {
    public class Program {
        static void Main(string[] args) {
            short gver = ServerConstants.GameVersion; // proc static constructor
            WzReaderTest.TestMob();
            WzReaderTest.TestSkill();
            WzReaderTest.TestField();
            WzReaderTest.TestItem();
        }
    }
}