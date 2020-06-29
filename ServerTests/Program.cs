using ServerTests.Wz;

namespace ServerTests {
    class Program {
        static void Main(string[] args) {
            // The first execution includes the time that the Just In Time (JIT) compiler spends converting the code from Microsoft's Intermediary Language (MSIL) into
            // the native executable machine code of whatever machine you're running the code on. All subsequent calls are re-using that already compiled code.
            // Each test case needs a fake execution or else the first measured execution time will be bloated significantly.

            SkillTest.Test();
        }
    }
}