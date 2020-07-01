using System;
using System.Collections.Generic;
using System.Net;
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

        public uint Id { get; set; }
        public string Username { get; set; }
        private string Password { get; set; }
        public byte Gender { get; set; }
        public User User { get; set; }
        public byte[] MachineId { get; set; }
        public string SecondaryPassword { get; set; }
        public IPAddress LastKnownIp { get; set; }

        public World World => Server.Worlds[_worldId];

        public Channel Channel => World.Channels[_channelId];

        public void SetWorld(byte worldId) {
            _worldId = worldId;
        }

        public void SetChannel(byte channelId) {
            _channelId = channelId;
        }

        public byte TryLogin(string password) {
            Password ??= password;
            byte result = 4;
            if (Password.Equals(password, StringComparison.Ordinal)) {
                result = 1;
            }

            return result;
        }
    }
}