using System;
using System.IO;
using System.Net.Sockets;
using Destiny.Security;
using NineToFive.IO;
using NineToFive.Security;

namespace NineToFive.Net {
    public class ClientSession : IDisposable {
        private Client _client;
        private TcpClient _socket;
        private readonly MapleCryptoHandler _cipher = new MapleCryptoHandler();
        private byte[] _packetBuffer = new byte[512];
        private int _packetSize;

        public ClientSession(Client client, TcpClient socket) {
            _client = client;
            _socket = socket;

            if (socket == null) return;
            _socket.GetStream().Write(_cipher.Initialize()); // send raw data
            BeginAccept();
        }

        public void Dispose() {
            _client = null;
            _socket?.Dispose();
            _socket = null;
            _cipher.Dispose();
        }

        private void BeginAccept() {
            try {
                _socket.GetStream().BeginRead(_packetBuffer, _packetSize, _packetBuffer.Length - _packetSize, OnReceivePacket, null);
            } catch (IOException) {
                Dispose();
            }
        }

        private void OnReceivePacket(IAsyncResult result) {
            lock (_cipher) {
                int count;
                try {
                    count = _socket.GetStream().EndRead(result);
                    if (count == 0) return; 
                } catch {
                    Dispose();
                    return;
                }

                // while sufficient information is present
                _packetSize += count;
                while (_packetSize >= 6) {
                    int packetLength = AesCryptograph.RetrieveLength(_packetBuffer) + 4; // packet + header

                    // expand the buffer because the full packet cannot fit
                    if (packetLength >= _packetBuffer.Length) {
                        byte[] buf = new byte[packetLength];
                        Buffer.BlockCopy(_packetBuffer, 0, buf, 0, _packetBuffer.Length);
                        _packetBuffer = buf;
                        break;
                    }

                    if (_packetSize >= packetLength) {
                        // sufficient information to process
                        _packetSize -= packetLength;
                        byte[] packet = new byte[packetLength];
                        Buffer.BlockCopy(_packetBuffer, 0, packet, 0, packetLength);

                        using Packet p = new Packet(_cipher.Decrypt(packet));
                        _client.ServerHandler.OnPacketReceived(_client, p);
                    } else break;
                }
            }

            BeginAccept();
        }

        /// <summary>
        /// encrypts and sends the specified byte array to the socket
        /// </summary>
        public void Write(byte[] b) {
            _socket.GetStream().Write(_cipher.Encrypt(b));
        }
    }
}