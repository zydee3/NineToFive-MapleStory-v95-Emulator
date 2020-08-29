using log4net;
using log4net.Config;
using NineToFive;
using NineToFive.Net.Interoperations;

[assembly: XmlConfigurator(ConfigFile = "logger-config.xml")]

namespace Central {
    public static class CentralServer {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CentralServer));

        public static void Main(string[] args) {
            Log.Info("Hello World, from Central Server!");
            
            Interoperability.ServerCreate(ServerConstants.InterCentralPort);
            Log.Info($"Interoperability listening on port {ServerConstants.InterCentralPort}");
        }
    }
}