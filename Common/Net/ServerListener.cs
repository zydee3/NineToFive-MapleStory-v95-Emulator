using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace NineToFive.Net {
    public abstract class ServerListener : IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerListener));
        protected readonly int Port;
        private TcpListener _listener;

        protected ServerListener(int port) {
            Port = port;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Dispose() {
            _listener?.Server.Dispose();
            _listener = null;
        }

        public void Start() {
            _listener.Start();
            _listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
        }

        private void OnAcceptTcpClient(IAsyncResult ar) {
            TcpClient socket = _listener.EndAcceptTcpClient(ar);
            try {
                ClientSession session = new ClientSession(this, socket);
                session.Client = new Client(session);
                Log.Info($"TCP connection established : {session.RemoteAddress}");
            } catch (IOException) {
                // probably disconnected when migrating sockets
            }

            _listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
        }

        public abstract void OnPacketReceived(Client c, Packet p);
    }
}