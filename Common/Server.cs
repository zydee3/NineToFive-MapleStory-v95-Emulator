using System;
using System.Collections.Generic;
using log4net;
using NineToFive.Game;
using MapleLib.WzLib;
using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;

namespace NineToFive {
    public static class Server {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));
        public static World[] Worlds { get; private set; }

        public static void Initialize() {
            Worlds = new World[ServerConstants.WorldCount];
            for (byte worldId = 0; worldId < Worlds.Length; worldId++) {
                World world = new World(worldId);
                world.Channels = new Channel[ServerConstants.ChannelCount];
                for (byte channelId = 0; channelId < world.Channels.Length; channelId++) {
                    int channelPort = ServerConstants.ChannelPort + channelId + (worldId * 100);
                    world.Channels[channelId] = new Channel(worldId, channelId, channelPort);
                }

                Worlds[worldId] = world;
                
                world.Templates = new Dictionary<uint, object>[Enum.GetNames(typeof(TemplateType)).Length];
                world.Fields = new Dictionary<uint, Field>[world.Channels.Length];
                world.Entities = new Dictionary<uint, Entity>[Enum.GetNames(typeof(EntityType)).Length];
                
                Log.Info($"Skeleton for world {(worldId + 1)} created with {world.Channels.Length} spooky channels");
            }
        }
    }
}