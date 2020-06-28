using System.Collections.Generic;
using System.Net.Sockets;
using NineToFive.Game;
using NineToFive.Net;

namespace NineToFive {
    public class Client {
        public readonly ServerListener Server;
        public readonly ClientSession Session;
        public readonly List<User> Users = new List<User>();

        public Client(ServerListener server, Socket socket) {
            Server = server;
            Session = new ClientSession(this, socket);

            Gender = 10;
        }

        public int Id { get; private set; }
        public string Username { get; set; }
        public byte Gender { get; set; }
        public bool Banned { get; set; }
        public byte[] MachineId { get; set; }

        public bool TryLogin(string password) {
            return true;
        }
    }
}