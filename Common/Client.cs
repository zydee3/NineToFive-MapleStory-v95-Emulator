using System.Collections.Generic;
using System.Net.Sockets;
using NineToFive.Game;
using NineToFive.Net;

namespace NineToFive {
    public class Client {
        public readonly ServerListener ServerHandler;
        public readonly ClientSession Session;
        public readonly List<User> Users = new List<User>();
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
        public World World => Server.Worlds[_worldId];
        public Channel Channel => World.Channels[_channelId];

        public byte TryLogin(string password) {
            return 1;
        }
    }
}