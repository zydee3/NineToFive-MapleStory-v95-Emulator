using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;

namespace NineToFive.Net {
    public abstract class ServerListener : IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServerListener));
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IPEndPoint _endPoint;
        private readonly Socket _socket;

        protected ServerListener(int port) {
            Port = port;
            _endPoint = new IPEndPoint(IPAddress.Any, port);
            _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
                Blocking = false,
                NoDelay = true,
            };
        }

        public int Port { get; }

        public void Dispose() {
            _cts.Cancel();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public async void Start() {
            _socket.Bind(_endPoint);
            _socket.Listen(50);
            while (true) {
                if (_cts.Token.IsCancellationRequested) break;
                ClientSession cs = null;
                try {
                    Socket socket = await _socket.AcceptAsync();
                    _socket.NoDelay = true;

                    cs = new ClientSession(this, socket);
                    cs.Client = new Client(cs);
                } catch (Exception e) {
                    Log.Info($"Connection closed : {e.InnerException}");
                    cs?.Dispose();
                }
            }
        }

        public abstract void OnPacketReceived(Client c, Packet p);
    }
}