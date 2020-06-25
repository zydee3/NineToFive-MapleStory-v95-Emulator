using NineToFive.Net;
using System.Net.Sockets;

namespace NineToFive {
    public class Client {
        public ServerListener Server { get; private set; }
        public ClientSession Session { get; private set; }
        public int ID { get; set; }
        public string Username { get; set; }

        public Client(ServerListener server, Socket socket) {
            Server = server;
            Session = new ClientSession(this, socket);
        }
    }
}
