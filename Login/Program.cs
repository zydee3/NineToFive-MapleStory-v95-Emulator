using System;
using log4net;
using NineToFive.Constants;
using NineToFive.Net;
using NineToFive.Net.Security;

namespace NineToFive.Login {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();

        static void Main(string[] args) {
            Log.Info("Hello World, from Login Server!");
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterLoginPort);
            Log.Info($"Interoperations listening on port {ServerConstants.InterLoginPort}");

            // Initialize the login server socket
            LoginServer server = new LoginServer(ServerConstants.LoginPort);
            server.Start();
            Log.Info($"Login server listening on port {server.Port}");

            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") {
                    return;
                }
            }
        }
    }
}