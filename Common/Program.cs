using log4net;
using NineToFive.Constants;
using NineToFive.Net;

namespace NineToFive {
    public class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args) {
            Log.Info("Hello World, from Central Server!");
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterCentralPort);
            Log.Info($"Interoperability listening on port {ServerConstants.InterCentralPort}");
        }
    }
}