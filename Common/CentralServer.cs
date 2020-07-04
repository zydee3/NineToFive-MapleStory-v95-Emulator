using log4net;
using log4net.Config;
using NineToFive.Constants;
using NineToFive.Net;

[assembly: XmlConfigurator(ConfigFile = "Resources/central-logger.xml")]

namespace NineToFive {
    class CentralServer {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CentralServer));

        static void Main(string[] args) {
            Log.Info("Hello World, from Central Server!");
            Interoperability.ServerCreate(ServerConstants.InterCentralPort);
            Log.Info($"Interoperability listening on port {ServerConstants.InterCentralPort}");
        }
    }
}