using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Interopation.Event;
using NineToFive.IO;
using NineToFive.Net.Security;

namespace NineToFive.Net {
    public class Interoperability {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Interoperability));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();
        private readonly TcpListener _server;

        public Interoperability(int port) {
            _server = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
        }

        private void OnInteroperationReceived(TcpClient c, Packet r) {
            Interoperation op = (Interoperation) r.ReadByte();
            Log.Info($"{op}");
            switch (op) {
                case Interoperation.ClientGenderUpdateRequest:
                    CentralServer.Clients[r.ReadString()].Gender = r.ReadByte();
                    return;
                case Interoperation.WorldInformationRequest:
                    c.GetStream().Write(SimpleCrypto.Encrypt(
                        WorldInformationRequest.OnHandle()));
                    return;
                case Interoperation.ChannelHostPermission:
                    c.GetStream().Write(SimpleCrypto.Encrypt(
                        ChannelHostRequest.OnHandle(r)));
                    return;
                case Interoperation.CheckPasswordRequest:
                    c.GetStream().Write(SimpleCrypto.Encrypt(
                        ClientAuthRequest.OnHandle(r)));
                    return;
                case Interoperation.CheckDuplicateIdRequest:
                    c.GetStream().Write(SimpleCrypto.Encrypt(
                        CheckDuplicateIdRequest.OnHandle(r)));
                    return;
                case Interoperation.ClientInitializeSPWRequest:
                    CentralServer.Clients[r.ReadString()].SecondaryPassword = r.ReadString();
                    return;
                case Interoperation.ChannelAddressRequest: {
                    using Packet w = new Packet();
                    Channel channel = Server.Worlds[r.ReadByte()].Channels[r.ReadByte()];
                    if (w.WriteBool(channel.HostAddress != null)) {
                        w.WriteBytes(channel.HostAddress.GetAddressBytes());
                    }

                    c.GetStream().Write(SimpleCrypto.Encrypt(w.ToArray()));
                    return;
                }
                case Interoperation.ChannelUserLimitRequest: {
                    World world = Server.Worlds[r.ReadByte()];
                    Channel channel = world.Channels[r.ReadByte()];
                    // request user count from the specified channel server
                    using Packet w = new Packet();
                    w.WriteByte((byte) Interoperation.ChannelUserLimitResponse);
                    w.WriteByte(world.Id);
                    w.WriteByte(channel.Id);
                    w.WriteBytes(GetPacketResponse(w.ToArray(), ServerConstants.InterChannelPort, channel.HostAddress.ToString()));

                    c.GetStream().Write(SimpleCrypto.Encrypt(w.ToArray()));
                    return;
                }
                case Interoperation.ChannelUserLimitResponse: {
                    World world = Server.Worlds[r.ReadByte()];
                    Channel channel = world.Channels[r.ReadByte()];
                    // respond to the central server with a user count of the specified channel
                    c.GetStream().Write(SimpleCrypto.Encrypt(
                        BitConverter.GetBytes(world.Users.Values.Count(u => u.Client.Channel == channel))));
                    return;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Immediately accepts connections from a new thread
        /// </summary>
        /// <param name="port">port to bind the socket</param>
        public static void ServerCreate(int port) {
            void StartSocket() {
                Interoperability instance = new Interoperability(port);
                instance._server.Start();
                while (true) ServerAcceptClients(instance);
            }

            new Thread(StartSocket).Start();
        }

        /// <summary>
        /// Handles the incoming connection
        /// </summary>
        private static void ServerAcceptClients(Interoperability instance) {
            try {
                using TcpClient client = instance._server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                // read packet length
                byte[] buffer = new byte[4];
                int count = stream.Read(buffer, 0, buffer.Length);
                if (count == 0) return;

                // read packet body
                buffer = new byte[BitConverter.ToInt32(buffer)];
                stream.Read(buffer, 0, buffer.Length);

                using Packet p = new Packet(buffer);
                instance.OnInteroperationReceived(client, p);
            } catch (SocketException) {
                // forcibly closed or smth
            } catch (Exception e) {
                Log.Error("Error while processing", e);
            }
        }

        public static bool TestConnection(IPAddress address, int port) {
            try {
                using TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(address, port));
                return client.Connected;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// sends a packet to the specified server in blocking mode, waiting for a packet response
        /// </summary>
        /// <param name="buffer">packet buffer to send</param>
        /// <param name="port">destination port on the specified server</param>
        /// <param name="address">ip address of the specified server</param>
        /// <returns>response information in regard to the packet sent, if no response</returns>
        public static byte[] GetPacketResponse(byte[] buffer, int port, string address = "127.0.0.1") {
            using TcpClient client = new TcpClient(address, port) {
                ReceiveTimeout = 15 // 15 second timeout is ok????
            };
            NetworkStream stream = client.GetStream();
            // send packet request
            buffer = SimpleCrypto.Encrypt(buffer);
            stream.Write(buffer, 0, buffer.Length);

            // get response length
            buffer = new byte[4];
            int count = stream.Read(buffer, 0, buffer.Length);
            if (count == 0) return null; // no response

            // read response packet
            buffer = new byte[BitConverter.ToInt32(buffer)];
            count = stream.Read(buffer);
            if (count == 0) throw new OperationCanceledException("Not enough data received");
            return buffer;
        }
    }
}