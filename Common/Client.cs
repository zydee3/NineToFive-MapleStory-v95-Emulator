using NineToFive.Net;
using System.Net.Sockets;

namespace NineToFive {
    public class Client {
        public ServerListener Server { get; }
        public ClientSession Session { get; }
        public int Id { get; set; }
        public string Username { get; set; }
        public byte Gender { get; set; }
        public bool Banned { get; set; }

        public Client(ServerListener server, Socket socket) {
            Server = server;
            Session = new ClientSession(this, socket);

            Gender = 10;
        }
    }
}
