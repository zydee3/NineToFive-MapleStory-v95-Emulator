using System;
using MapleLib.WzLib;

namespace Wz {
    class Program {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args) {
            Logger.Info("Test");
        }
    }
}