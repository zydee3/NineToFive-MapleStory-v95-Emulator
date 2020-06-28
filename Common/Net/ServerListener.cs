using System;
using System.Net;
using System.Net.Sockets;
using NineToFive.IO;

namespace NineToFive.Net {
    public abstract class ServerListener : IDisposable {
        private Socket Listener { get; set; }
        private readonly int _port;

        public ServerListener(int port) {
            _port = port;
        }

        public void Start() {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // allow incoming connections
            Listener.Bind(new IPEndPoint(IPAddress.Any, _port));
            Listener.Listen(15);
            // accept an incoming connection
            Listener.BeginAccept(OnSocketConnect, null);
        }

        private void OnSocketConnect(IAsyncResult result) {
            Socket socket = Listener.EndAccept(result);
            _ = new Client(this, socket);
            // accept the next incoming connection 
            Listener.BeginAccept(OnSocketConnect, null);
        }

        public void Dispose() {
            Listener?.Close();
            Listener = null;
        }
        
        public abstract void OnPacketReceived(Client c, Packet p);
    }
}