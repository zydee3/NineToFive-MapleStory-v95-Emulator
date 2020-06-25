using Destiny.Security;
using NineToFive.IO;
using System;
using System.Net.Sockets;

namespace NineToFive.Net {
    public class ClientSession : IDisposable {
        private Client Client { get; set; }
        private Socket Socket { get; set; }
        public MapleCryptoHandler Cipher { get; set; }
        private Packet ByteBuffer { get; set; }

        public ClientSession(Client client, Socket socket) {
            Client = client;
            Socket = socket;
            Cipher = new MapleCryptoHandler();
            ByteBuffer = new Packet() { PacketAccess = PacketAccess.Read };

            Socket.Send(Cipher.Initialize()); // send raw data
            BeginAccept(ByteBuffer.Capacity); // accept as much information possible at once 
        }

        public void Dispose() {
            Cipher.Dispose();
            Socket.Dispose();
            ByteBuffer.Dispose();
        }

        private void BeginAccept(int size) {
            // accept incoming data and store it inside ByteBuffer.Array
            Socket.BeginReceive(ByteBuffer.Array, ByteBuffer.Position, size,
                SocketFlags.None, new AsyncCallback(OnReceivePacket), null);
        }

        private void OnReceivePacket(IAsyncResult result) {
            int count = Socket.EndReceive(result);
            if (count == 0) return;

            // while sufficient information is present
            ByteBuffer.Size += count;
            while (ByteBuffer.Size >= 6) {
                byte[] header = ByteBuffer.ReadBytes(4);
                ByteBuffer.Position -= 4; // we are only peeking information

                int packetLength = AesCryptograph.RetrieveLength(header) + 4; // packet + header
                ByteBuffer.TryGrow(packetLength); // make sure packet can fit in buffer

                // sufficient information to process
                if (ByteBuffer.Available >= packetLength) {
                    // read bytes then move stream position back, to ready for new information
                    byte[] packet = ByteBuffer.ReadBytes(packetLength);
                    ByteBuffer.Position -= packetLength;
                    // remove information from the buffer
                    ByteBuffer.Size -= packetLength;

                    using Packet p = new Packet(Cipher.Decrypt(packet));
                    Client.Server.OnPacketReceived(Client, p);
                } else {
                    // insufficient information; re-accept data but only what's necessary
                    BeginAccept(packetLength - ByteBuffer.Size);
                    return;
                }
            }
            BeginAccept(ByteBuffer.Capacity);
        }

        /// <summary>
        /// encrypts and sends the specified byte array to the socket
        /// </summary>
        public void Write(byte[] b) {
            Socket.Send(Cipher.Encrypt(b));
        }
    }
}
