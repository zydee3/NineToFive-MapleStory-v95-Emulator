using System;
using System.Net.Sockets;
using Destiny.Security;
using NineToFive.IO;
using NineToFive.Security;

namespace NineToFive.Net {
    public class ClientSession : IDisposable {
        private Client _client;
        private Socket _socket;
        private readonly MapleCryptoHandler _cipher = new MapleCryptoHandler();
        private byte[] _packetBuffer = new byte[512];
        private int _packetSize;

        public ClientSession(Client client, Socket socket) {
            _client = client;
            _socket = socket;

            _socket.Send(_cipher.Initialize()); // send raw data
            BeginAccept();
        }

        public void Dispose() {
            _client = null;
            _socket?.Dispose();
            _socket = null;
            _cipher.Dispose();
        }

        private void BeginAccept() {
            lock (_cipher) {
                _socket.BeginReceive(_packetBuffer, _packetSize, _packetBuffer.Length - _packetSize,
                    SocketFlags.None, OnReceivePacket, null);
            }
        }

        private void OnReceivePacket(IAsyncResult result) {
            lock (_cipher) {
                int count;
                try {
                    count = _socket.EndReceive(result);
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
            _socket.Send(_cipher.Encrypt(b));
        }
    }
}