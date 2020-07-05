using System;
using NineToFive.Game.Entity;
using ServerTests.Wz;

namespace ServerTests {
    class Program {
        private const bool 
            _field = false,
            _item  = false,
            _mob   = false,
            _skill = false;
        
        static void Main(string[] args) {
            // The first execution includes the time that the Just In Time (JIT) compiler spends converting the code from Microsoft's Intermediary Language (MSIL) into
            // the native executable machine code of whatever machine you're running the code on. All subsequent calls are re-using that already compiled code.
            // Each test case needs a fake execution or else the first measured execution time will be bloated significantly.
            
            if(_field) FieldTest.Test();
            if(_item)  ItemTest.Test();
            if(_mob)   MobTest.Test();
            if(_skill) SkillTest.Test();
        }
    }
}