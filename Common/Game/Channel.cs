using NineToFive.Net;

namespace NineToFive.Game {
    public class Channel {
        private readonly byte _worldId;
        public readonly ChannelSnapshot Snapshot = new ChannelSnapshot();

        public Channel(byte worldId, byte id, int port) {
            _worldId = worldId;
            Id = id;
            Port = port;
        }

        public World World => Server.Worlds[_worldId];
        public byte Id { get; }
        public int Port { get; }
        public ServerListener ServerListener { get; set; }
    }

    /// <summary>
    /// container for data the login server requires from the channel servers
    /// </summary>
    public class ChannelSnapshot {
        /// <summary>
        /// If the channel already has a server host
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Last recorded value of user count 
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// IP address of the server host
        /// </summary>
        public string HostAddress { get; set; }
    }
}