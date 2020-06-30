using System.Collections.Generic;
using System.Net.Sockets;
using NineToFive.Game;
using NineToFive.Net;

namespace NineToFive {
    public class Client {
        public readonly ServerListener ServerHandler;
        public readonly ClientSession Session;
        public readonly List<User> Users = new List<User>(15);
        private byte _worldId, _channelId;

        public Client(ServerListener server, TcpClient socket) {
            ServerHandler = server;
            Session = new ClientSession(this, socket);

            Gender = 10;
        }

        public int Id { get; private set; }
        public string Username { get; set; }
        public byte Gender { get; set; }
        public User User { get; set; }
        public byte[] MachineId { get; set; }

        public byte LoginOption { get; set; }

        public World World => Server.Worlds[_worldId];

        public Channel Channel => World.Channels[_channelId];

        public void SetWorld(byte worldId) {
            _worldId = worldId;
        }

        public void SetChannel(byte channelId) {
            _channelId = channelId;
        }

        public byte TryLogin(string password) {
            return 1;
        }
    }
}