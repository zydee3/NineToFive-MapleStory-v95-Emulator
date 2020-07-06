using log4net;
using log4net.Config;
using NineToFive.Constants;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Login {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args) {
            Log.Info("Hello World, from Login Server!");
            Interoperability.ServerCreate(ServerConstants.InterLoginPort);
            Log.Info($"Interoperations listening on port {ServerConstants.InterLoginPort}");

            // Initialize the login server socket
            LoginServer server = new LoginServer(ServerConstants.LoginPort);
            server.Start();
            Log.Info($"Login server listening on port {server.Port}");
        }
    }
}