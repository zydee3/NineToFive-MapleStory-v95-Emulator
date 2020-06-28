using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using log4net;
using log4net.Config;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.Net.Security;

[assembly: XmlConfigurator(ConfigFile = "login-logger.xml")]

namespace NineToFive.Login {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();

        static void Main(string[] args) {
            Log.Info("Hello World, from Login Server!");
            // Initialize the skeleton of the server (worlds, channels, config, etc.)
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterLoginPort, OnInteroperationReceived);
            Log.Info($"Interoperations listening on port {ServerConstants.InterLoginPort}");

            // Initialize the login server socket
            LoginServer server = new LoginServer(ServerConstants.LoginPort);
            server.Start();
            Log.Info($"Login server listening on port {ServerConstants.LoginPort}");

            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") {
                    return;
                }
            }
        }

        private static void OnInteroperationReceived(TcpClient c, Packet p) {
            Interoperations op = (Interoperations) p.ReadByte();
            switch (op) {
                case Interoperations.ChannelHostPermission: {
                    IPAddress address = new IPAddress(p.ReadBytes(4));
                    World world = Server.Worlds[p.ReadByte()];
                    byte min = p.ReadByte(), max = p.ReadByte();
                    byte[] response = new byte[1];
                    Log.Info($"Server {address} is asking to host channels {min} through {max} in world {world.Id}");
                    if (max >= world.Channels.Length) {
                        // index out of bounds
                        c.GetStream().Write(SimpleCrypto.Encrypt(response));
                        return;
                    }

                    foreach (Channel channel in world.Channels) {
                        if (!channel.Snapshot.IsActive) continue;
                        // send a tiny packet to check the connection of the server
                        if (!Interoperability.TestConnection(channel.Snapshot.HostAddress, channel.Port)) {
                            // test failed so assume server is offline
                            Log.Info($"Connection test to channel {channel.Id} in world {world.Id} failed. Channel is now available for hosting");
                            channel.Snapshot.IsActive = false;
                            channel.Snapshot.HostAddress = null;
                        }
                    }

                    if (world.Channels.Any(ch => ch.Snapshot.IsActive && ch.Id >= min && ch.Id <= max)) {
                        // channel is already being hosted
                        c.GetStream().Write(SimpleCrypto.Encrypt(response));
                    }

                    response[0] = 1; // granted permission
                    for (int i = min; i <= max; i++) {
                        ChannelSnapshot snapshot = world.Channels[i].Snapshot;
                        snapshot.IsActive = true;
                        snapshot.HostAddress = address.ToString();
                        Log.Info($"Channel {i} is now being hosted by server {snapshot.HostAddress}");
                    }

                    c.GetStream().Write(SimpleCrypto.Encrypt(response));
                    return;
                }
            }
        }
    }
}