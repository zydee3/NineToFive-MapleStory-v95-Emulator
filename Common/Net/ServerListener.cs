using NineToFive.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace NineToFive.Net {
    public abstract class ServerListener : IDisposable {
        private Socket Listener { get; set; }
        private readonly int Port;

        public ServerListener(int port) {
            Port = port;

            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // allow incoming connections
            Listener.Bind(new IPEndPoint(IPAddress.Any, Port));
            Listener.Listen(15);
            // accept an incoming connection
            Listener.BeginAccept(OnSocketConnect, null);
        }

        private void OnSocketConnect(IAsyncResult result) {
            Socket socket = Listener.EndAccept(result);
            Client user = new Client(this, socket);
            // accept the next incoming connection 
            Listener.BeginAccept(OnSocketConnect, null);
        }

        public void Dispose() {
            Listener.Close();
        }

        public abstract void OnPacketReceived(Client c, Packet p);
    }
}
