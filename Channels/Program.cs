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

[assembly: XmlConfigurator(ConfigFile = "channel-logger.xml")]

namespace NineToFive.Channels {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();

        static void Main(string[] args) {
            Log.Info("Hello World, from Channel Server!");
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterChannelPort, OnInteroperationReceived);
            Log.Info($"Interoperations listening on port {ServerConstants.InterLoginPort}");

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
                if (input == "exit") {
                    return;
                }
            }
        }

        private static void OnInteroperationReceived(TcpClient c, Packet p) {
            Interoperations op = (Interoperations) p.ReadByte();
            switch (op) {
                case Interoperations.WorldInformationRequest: {
                    Log.Info($"Received interoperation request for world information");
                    World world = Server.Worlds[p.ReadByte()];
                    using Packet w = new Packet();
                    foreach (Channel channel in world.Channels) {
                        if (channel.ServerListener == null) continue;
                        w.WriteByte(channel.Id);
                        w.WriteInt(world.Users.Values.Count(u => u.Client.Channel.Id == channel.Id));
                    }

                    w.WriteByte(255);
                    c.GetStream().Write(SimpleCrypto.Encrypt(w.ToArray()));
                    return;
                }
            }
        }

        static void InitializeChannels(ref string[] args) {
            if (!byte.TryParse(args[0], out byte min) || !byte.TryParse(args[1], out byte max)) {
                throw new ArgumentException($"NaN {args[0]} or {args[1]}");
            }

            World world = Server.Worlds[World.ActiveWorld];
            Log.Info($"Asking for permission to host channels from {min} to {max} in world {world.Id}");
            byte[] address = IPAddress.Parse(ServerConstants.Address).GetAddressBytes();
            if (!Interoperability.SendChannelHostPermission(ServerConstants.InterLoginPort, address, world.Id, min, max)) {
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