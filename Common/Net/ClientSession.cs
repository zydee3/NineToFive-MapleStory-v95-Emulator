using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Destiny.Security;
using log4net;
using NineToFive.Security;
using NineToFive.SendOps;

namespace NineToFive.Net {
    public class ClientSession : IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientSession));

        private MapleCryptoHandler _cipher = new MapleCryptoHandler();
        private ServerListener _server;
        private TcpClient _socket;

        private readonly byte[] _packetHeader = new byte[4];
        private byte[] _packetBody = new byte[1024];

        public ClientSession(ServerListener server, TcpClient socket) {
            _server = server;
            _socket = socket;

            _socket.NoDelay = true;
            RemoteAddress = ((IPEndPoint) socket.Client.RemoteEndPoint).Address;

            // handshake data
            _socket.GetStream().Write(_cipher.Initialize());
            BeginReadPacketHeader();
        }

        public IPAddress RemoteAddress { get; }
        public Client Client { get; set; }

        public void Dispose() {
            Client?.Dispose();
            Client = null;

            _socket?.Dispose();
            _socket = null;

            _cipher?.Dispose();
            _cipher = null;

            _server = null;
        }

        private void DisposeIfNecessary() {
            if (_socket?.Connected == true) return;
            if (Client != null) {
                var now = DateTime.Now.TimeOfDay;
                if (now - Client.PingTimestamp > TimeSpan.FromSeconds(30)) {
                    using Packet w = new Packet();
                    w.WriteShort((short) CClientSocket.OnAliveReq);
                    Write(w.ToArray());
                }
            }
            Dispose();
        }

        private void BeginReadPacketHeader() {
            DisposeIfNecessary();
            try {
                _socket.GetStream().BeginRead(_packetHeader, 0, _packetHeader.Length, EndReadPacketHeader, null);
            } catch (IOException) {
                Dispose();
            }
        }

        private void EndReadPacketHeader(IAsyncResult ar) {
            DisposeIfNecessary();
            try {
                _socket.GetStream().EndRead(ar);
            } catch (IOException) {
                // disconnection
                Dispose();
                return;
            }

            if (!_cipher.De.IsValidPacket(_packetHeader)) {
                Log.Warn("Invalid packet received");
                Dispose();
                return;
            }

            int packetLength = AesCryptograph.RetrieveLength(_packetHeader);
            if (packetLength > _packetBody.Length) {
                _packetBody = new byte[packetLength];
            }

            _socket.GetStream().BeginRead(_packetBody, 0, packetLength, BeginReadPacketBody, packetLength);
        }

        private void BeginReadPacketBody(IAsyncResult ar) {
            DisposeIfNecessary();
            try {
                _socket.GetStream().EndRead(ar);
            } catch (IOException) {
                // disconnection
                Dispose();
                return;
            }

            int packetLength = (int) ar.AsyncState;
            byte[] packet = new byte[packetLength];
            Buffer.BlockCopy(_packetBody, 0, packet, 0, packetLength);
            try {
                Packet p = new Packet(_cipher.Decrypt(packet));
                _server.OnPacketReceived(Client, p);
            } catch (Exception e) {
                Log.Error("Failed to handle packet", e);
            }

            BeginReadPacketHeader();
        }

        /// <summary>
        /// encrypts then sends the byte array to the client socket stream
        /// </summary>
        public void Write(byte[] b) {
            DisposeIfNecessary();
            try {
                _socket.GetStream().Write(_cipher.Encrypt(b));
            } catch (IOException) {
                Dispose();
                // An existing connection was forcibly closed by the remote host.    
            }
        }
    }
}