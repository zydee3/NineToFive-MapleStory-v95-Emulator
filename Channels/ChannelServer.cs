﻿using System;
using System.Linq;
using System.Net;
using log4net;
using log4net.Config;
using NineToFive.Event;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;
using NineToFive.Net.Interoperations;

[assembly: XmlConfigurator(ConfigFile = "logger-config.xml")]

namespace NineToFive {
    public sealed class ChannelServer : ServerListener {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChannelServer));
        private readonly EventDirector _director;

        public ChannelServer(int port) : base(port) {
            _director = new EventDirector {
                [(short) ReceiveOperations.Login_OnEnterGamePacket] = typeof(CharEnterGameEvent),

                [(short) ReceiveOperations.OnTransferFieldRequest] = typeof(TransferFieldEvent),

                [(short) ReceiveOperations.CWvsContext_OnStatChanged] = typeof(AbilityUpEvent),
                
                [(short) ReceiveOperations.User_OnUserMove] = typeof(UserMoveEvent),
                [(short) ReceiveOperations.User_OnChatMsg] = typeof(ChatMsgEvent),
                [(short) ReceiveOperations.User_OnEmotion] = typeof(EmotionChangeEvent),

                [(short) ReceiveOperations.UserLocal_OnPortalCollision] = typeof(PortalCollisionEvent),
                [(short) ReceiveOperations.UserLocal_TryRegisterTeleport] = typeof(RegisterTeleportEvent),
                
                [(short) ReceiveOperations.Field_LogChatMsgSlash]  = typeof(ChatMsgSlashEvent),
                [(short) ReceiveOperations.Field_SendChatMsgSlash] = typeof(ChatMsgSlashEvent),
            };
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!_director.Events.TryGetValue(operation, out Type t)) {
                Log.Info($"[unhandled] {operation} (0x{operation:X2}) : {p.ToArrayString(true)}");
                Log.Info($"[ascii-decode] {p}");
                Log.Info("-----------");
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                Log.Info($"Handled operation : {operation} | 0x{operation:X2} : {handler.GetType().Name}");
                try {
                    if (handler.OnProcess(p)) {
                        handler.OnHandle();
                    }
                } catch (Exception e) {
                    try {
                        handler.OnError(e);
                    } catch {
                        // ignore
                    }
                }
            }
        }

        public static void Main(string[] args) {
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

        private static void InitializeChannels(ref string[] args) {
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