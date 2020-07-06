using System;
using System.Collections.Generic;
using log4net;
using NineToFive.Game;

namespace NineToFive {
    public static class Server {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));
        public static readonly Dictionary<string, Client> Clients = new Dictionary<string, Client>(StringComparer.OrdinalIgnoreCase);
        public static World[] Worlds { get; }

        static Server() {
            Worlds = new World[ServerConstants.WorldCount];
            for (byte worldId = 0; worldId < Worlds.Length; worldId++) {
                World world = new World(worldId);
                world.Channels = new Channel[ServerConstants.ChannelCount];
                for (byte channelId = 0; channelId < world.Channels.Length; channelId++) {
                    int channelPort = ServerConstants.ChannelPort + channelId + (worldId * 100);
                    world.Channels[channelId] = new Channel(worldId, channelId, channelPort);
                }

                Worlds[worldId] = world;
                Log.Info($"Skeleton for world {(worldId + 1)} created with {world.Channels.Length} spooky channels");
            }
        }

        public static Client AddClientIfAbsent(string username) {
            Clients.TryGetValue(username, out Client client);
            if (client != null) return client;

            client = new Client();
            Clients.Add(username, client);
            return client;
        }
    }
}