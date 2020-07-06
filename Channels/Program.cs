using System;
using System.Linq;
using System.Net;
using log4net;
using log4net.Config;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.Net.Security;

namespace NineToFive.Channels {
    class Program {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args) {
            if (!Interoperability.TestConnection(IPAddress.Parse(ServerConstants.CentralServer), ServerConstants.InterCentralPort)) {
                Log.Info("Central server is not online. Channel server may not start.");
                return;
            }
            
            Log.Info("Hello World, from Channel Server!");
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

            if (Server.Worlds[World.ActiveWorld].Channels.Count(ch => ch.ServerListener != null) == 0) {
                Log.Warn("Invalid usage. Please specify a --channels and --world cmd-line argument");
                Environment.Exit(0);
            }
        }

        static void InitializeChannels(ref string[] args) {
            if (!byte.TryParse(args[0], out byte min) || !byte.TryParse(args[1], out byte max)) {
                throw new ArgumentException($"NaN {args[0]} or {args[1]}");
            }

            World world = Server.Worlds[World.ActiveWorld];
            Log.Info($"Asking for permission to host channels from {min} to {max} in world {world.Id}");
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.ChannelHostRequest);
            w.WriteBytes(IPAddress.Parse(ServerConstants.HostServer).GetAddressBytes()); 
            w.WriteByte(world.Id);
            w.WriteByte(min);
            w.WriteByte(max);
            if (Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort)[0] != 1) {
                throw new OperationCanceledException($"Denied permission to host channels from {min} to {max} in world {world.Id}");
            }

            Log.Info("Permission to host specified channels granted");

            for (int i = min; i <= max; i++) {
                Channel channel = world.Channels[i];
                (channel.ServerListener = new ChannelServer(channel.Port)).Start();
                Log.Info($"World {channel.World.Id} Channel {channel.Id} listening on port {channel.Port}");
            }
        }
    }
}