using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using NineToFive.Net.Security;
using NineToFive.Util;

namespace NineToFive.Net {
    public class ClientSession : IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientSession));
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly object _lock = new object();

        private readonly byte[] _packetHeader = new byte[4];

        private uint _seqSnd, _seqRcv;
        private ServerListener _server;
        private Socket _socket;

        public ClientSession(ServerListener server, Socket socket) {
            _server = server;
            _socket = socket;

            using Packet packet = new Packet();
            packet.WriteShort(14);
            packet.WriteShort(ServerConstants.GameVersion);
            packet.WriteString("1");
            _seqRcv = packet.WriteUInt(Randomizer.GetUInt());
            _seqSnd = packet.WriteUInt(Randomizer.GetUInt());
            packet.WriteByte(8);

            _socket.SendAsync(packet.ToArray(), SocketFlags.None, _cts.Token);

            RemoteAddress = ((IPEndPoint) socket.RemoteEndPoint).Address;
            Log.Info($"Connection established : {RemoteAddress}");

            try {
                _socket.BeginReceive(_packetHeader, 0, _packetHeader.Length, SocketFlags.None, DoPacketReceive, null);
            } catch (SocketException e) {
                Log.Info($"Connection closed : {e.Message}");
                Dispose();
            }
        }

        private async void DoPacketReceive(IAsyncResult ar) {
            try {
                if (_socket.EndReceive(ar) == 0) {
                    _cts.Cancel();
                    Log.Info($"Connection closed (empty packet): {RemoteAddress}");
                }
            } catch {
                _cts.Cancel();
            }

            if (ShouldDispose()) {
                Dispose();
                return;
            }

            byte[] packet;
            ushort seq;
            using (Packet r = new Packet(_packetHeader)) {
                seq = r.ReadUShort();
                var len = (ushort) (r.ReadShort() ^ seq);
                packet = new byte[len];
            }

            await _socket.ReceiveAsync(packet, SocketFlags.None, _cts.Token);

            var version = (short) ((_seqRcv >> 16) ^ seq);
            if (version != -96 && version != 95) {
                Log.Info($"Unknown data: {RemoteAddress} - {version}");
                Dispose();
                return;
            }

            AesCipher.Transform(packet, _seqRcv);
            ShandaCipher.Decrypt(packet);
            _seqRcv = IGCipher.InnoHash(_seqRcv, 4, 0);

            using (Packet p = new Packet(packet)) {
                _server.OnPacketReceived(Client, p);
            }

            if (ShouldDispose()) {
                Log.Info($"Connection closed (disconnected): {RemoteAddress}");
                Dispose();
                return;
            }

            _socket.BeginReceive(_packetHeader, 0, _packetHeader.Length, SocketFlags.None, DoPacketReceive, null);
        }

        public IPAddress RemoteAddress { get; }
        public Client Client { get; set; }

        public void Dispose() {
            lock (_lock) {
                _cts.Cancel();

                Client?.Dispose();
                Client = null;

                _socket?.Dispose();
                _socket = null;

                _server = null;
            }
        }

        public bool ShouldDispose() {
            if (Client != null) {
                var then = Client.PingTimestamp;
                var now = DateTime.Now.TimeOfDay;
                var elapsed = now - then;

                if (elapsed < TimeSpan.FromSeconds(120)) return false;
                if (then > TimeSpan.Zero) {
                    Log.Warn($"{RemoteAddress} timed out. Last successful ping was {elapsed} ago.");
                }
            }

            return _socket == null || !_socket.Connected || _cts.IsCancellationRequested;
        }

        public void Write(byte[] b) {
            if (ShouldDispose()) {
                Log.Info($"Connection closed(3): {RemoteAddress}");
                Dispose();
                return;
            }

            var ver = (short) ((_seqSnd >> 16) ^ -96);
            var len = (short) (b.Length ^ ver);

            ShandaCipher.Encrypt(b);
            AesCipher.Transform(b, _seqSnd);

            using Packet w = new Packet();
            w.WriteShort(ver);
            w.WriteShort(len);
            w.WriteBytes(b);
            _socket.SendAsync(w.ToArray(), SocketFlags.None, _cts.Token);

            _seqSnd = IGCipher.InnoHash(_seqSnd, 4, 0);
        }
    }
}