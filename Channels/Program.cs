using System;
using System.Net;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Net;
using NineToFive.Net.Security;

namespace NineToFive.Channels {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();

        static void Main(string[] args) {
            Log.Info("Hello World, from Channel Server!");
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterChannelPort);
            Log.Info($"Interoperations listening on port {ServerConstants.InterChannelPort}");

            foreach (string arg in args) {
                if (arg.StartsWith("--channels")) {
                    string[] channels = arg.Split("=")[1].Split("-");
                    InitializeChannels(ref channels);
                } else if (arg.StartsWith("--world")) {
                    World.ActiveWorld = byte.Parse(arg.Split("=")[1]);
                }
            }

            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") return;
                // no reason to log a console message
                Console.WriteLine("I only know the command: \"exit\"");
            }
        }

        static void InitializeChannels(ref string[] args) {
            if (!byte.TryParse(args[0], out byte min) || !byte.TryParse(args[1], out byte max)) {
                throw new ArgumentException($"NaN {args[0]} or {args[1]}");
            }

            World world = Server.Worlds[World.ActiveWorld];
            Log.Info($"Asking for permission to host channels from {min} to {max} in world {world.Id}");
            byte[] address = IPAddress.Parse(ServerConstants.Address).GetAddressBytes();
            if (!Interoperability.SendChannelHostPermission(address, world.Id, min, max)) {
                throw new OperationCanceledException($"Denied permission to host channels from {min} to {max}");
            }

            Log.Info("Permission to host specified channels granted");

            for (int i = min; i <= max; i++) {
                Channel channel = world.Channels[i];
                (channel.ServerListener = new ChannelServer(channel.Port)).Start();
                channel.Snapshot.IsActive = true;
                Log.Info($"World {channel.World.Id} Channel {channel.Id} listening on port {channel.Port}");
            }
        }
    }
}