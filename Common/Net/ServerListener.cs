using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NineToFive.Net {
    public abstract class ServerListener : IDisposable {
        public readonly int Port;
        private TcpListener _listener;

        public ServerListener(int port) {
            Port = port;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Dispose() {
            _listener?.Server.Dispose();
            _listener = null;
        }

        public void Start() {
            _listener.Start();

            void StartSocket() {
                while (true) ServerAcceptClient();
            }

            new Thread(StartSocket).Start();
        }

        private void ServerAcceptClient() {
            TcpClient client = _listener.AcceptTcpClient();
            _ = new Client(this, client);
        }

        public abstract void OnPacketReceived(Client c, Packet p);
    }
}