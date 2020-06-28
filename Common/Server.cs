using log4net;
using NineToFive.Game;

namespace NineToFive {
    public static class Server {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));
        public static World[] Worlds { get; private set; }

        public static void Initialize() {
            Worlds = new World[Constants.ServerConstants.WorldCount];
            for (byte worldId = 0; worldId < Worlds.Length; worldId++) {
                World world = new World(worldId);
                world.Channels = new Channel[Constants.ServerConstants.ChannelCount];
                for (byte channelId = 0; channelId < world.Channels.Length; channelId++) {
                    int channelPort = Constants.ServerConstants.ChannelPort + channelId + (worldId * 100);
                    world.Channels[channelId] = new Channel(worldId, channelId, channelPort);
                }

                Worlds[worldId] = world;
                Log.Info($"Skeleton for world {(worldId + 1)} created with {world.Channels.Length} spooky channels");
            }
        }
    }
}