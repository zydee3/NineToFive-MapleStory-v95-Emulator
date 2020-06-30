using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net.Security;
using NineToFive.ReceiveOps;

namespace NineToFive.Net {
    public class Interoperability {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Interoperability));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();
        private readonly TcpListener _server;

        public Interoperability(int port) {
            _server = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
        }

        #region Server methods

        private void OnInteroperationReceived(TcpClient c, Packet p) {
            Interoperations op = (Interoperations) p.ReadByte();
#if DEBUG
            Log.Info($"Interoperation received : {op}");
#endif
            switch (op) {
                default:                                  throw new ArgumentOutOfRangeException();
                case Interoperations.CheckConnectionTest: return;
                case Interoperations.WorldInformationRequest: {
                    using Packet w = new Packet();
                    foreach (World world in Server.Worlds) {
                        w.WriteByte(world.Id);
                        // find only channels that have a listening socket on this server
                        foreach (Channel channel in world.Channels.Where(ch => ch.ServerListener != null)) {
                            w.WriteByte(channel.Id);
                            w.WriteInt(world.Users.Values.Count(u => u.Client.Channel.Id == channel.Id));
                        }

                        w.WriteByte(255);
                    }

                    w.WriteByte(255);

                    c.GetStream().Write(SimpleCrypto.Encrypt(w.ToArray()));
                    return;
                }
                case Interoperations.ChannelHostPermission: {
                    IPAddress address = new IPAddress(p.ReadBytes(4));
                    World world = Server.Worlds[p.ReadByte()];
                    byte min = p.ReadByte(), max = p.ReadByte();
                    Log.Info($"Server {address} is asking to host channels {min} through {max} in world {world.Id}");

                    byte[] response = new byte[1];
                    if (max >= world.Channels.Length) {
                        // index out of bounds
                        c.GetStream().Write(SimpleCrypto.Encrypt(response));
                        return;
                    }

                    foreach (Channel channel in world.Channels.Where(ch => ch.Snapshot.IsActive)) {
                        // send a tiny packet to check the connection of the server
                        if (!TestConnection(channel.HostAddress, channel.Port)) {
                            // test failed so assume server is offline
                            Log.Info($"Connection test to channel {channel.Id} in world {world.Id} failed. Channel is now available for hosting");
                            channel.Snapshot.IsActive = false;
                            channel.HostAddress = null;
                        }
                    }

                    if (world.Channels.Any(ch => ch.Snapshot.IsActive && ch.Id >= min && ch.Id <= max)) {
                        // channel is already being hosted
                        c.GetStream().Write(SimpleCrypto.Encrypt(response));
                        return;
                    }

                    response[0] = 1; // granted permission
                    for (int i = min; i <= max; i++) {
                        Channel channel = world.Channels[i];
                        ChannelSnapshot snapshot = channel.Snapshot;
                        snapshot.IsActive = true;
                        channel.HostAddress = address;
                        Log.Info($"Channel {i} is now being hosted by server {channel.HostAddress}");
                    }

                    c.GetStream().Write(SimpleCrypto.Encrypt(response));
                    return;
                }
                case Interoperations.CheckPasswordRequest: {
                    string username = p.ReadString();
                    string password = p.ReadString();
                    CentralServer.Clients.TryGetValue(username, out Client client);
                    if (client == null) {
                        client = new Client(null, null) {Username = username};
                        CentralServer.Clients.Add(username, client);
                    }

                    byte response = client.TryLogin(password);
                    c.GetStream().Write(SimpleCrypto.Encrypt(new[] {response}));
                    return;
                }
                case Interoperations.CheckDuplicateIdRequest: {
                    string username = p.ReadString();
                    c.GetStream().Write(SimpleCrypto.Encrypt(new byte[] {0}));
                    return;
                }
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
                stream.Read(buffer, 0, buffer.Length);

                // read packet body
                buffer = new byte[BitConverter.ToInt32(buffer)];
                stream.Read(buffer, 0, buffer.Length);

                using Packet p = new Packet(buffer);
                instance.OnInteroperationReceived(client, p);
            } catch (Exception e) {
                Log.Error("Error while processing", e);
            }
        }

        #endregion

        /// <summary>
        /// Sends a request for permission to host the specified channels
        /// <para>Should always assume the <paramref name="address"/> parameter is IPv4</para>
        /// </summary>
        /// <param name="address">remote address of the asking server</param>
        /// <param name="worldId">id of world which the channels belong to</param>
        /// <param name="min">lower bound channel</param>
        /// <param name="max">upper bound channel</param>
        /// <returns></returns>
        public static bool SendChannelHostPermission(byte[] address, byte worldId, byte min, byte max) {
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperations.ChannelHostPermission);
            w.WriteBytes(address);
            w.WriteByte(worldId);
            w.WriteByte(min);
            w.WriteByte(max);
            return GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort)?[0] == 1;
        }

        public static bool TestConnection(IPAddress address, int port) {
            try {
                using TcpClient client = new TcpClient(new IPEndPoint(address, port));
                return true;
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
        /// <returns>response information in regard to the packet sent (no <see cref="Interoperations"/> header is provided)</returns>
        public static byte[] GetPacketResponse(byte[] buffer, int port, string address = "127.0.0.1") {
            using TcpClient client = new TcpClient(address, port);
            NetworkStream stream = client.GetStream();
            // send packet request
            buffer = SimpleCrypto.Encrypt(buffer);
            stream.Write(buffer, 0, buffer.Length);

            // get response length
            buffer = new byte[4];
            int count = stream.Read(buffer, 0, buffer.Length);
            if (count == 0) throw new OperationCanceledException("Not enough data received");

            // read response packet
            buffer = new byte[BitConverter.ToInt32(buffer)];
            count = stream.Read(buffer);
            if (count == 0) throw new OperationCanceledException("Not enough data received");
            return buffer;
        }
    }

    public enum Interoperations : byte {
        /// <summary>
        /// When the channel server is requesting the login server for permission
        /// to create sockets for specific channels in world 
        /// </summary>
        ChannelHostPermission = 0,

        /// <summary>
        /// Login server request for world information (user count, message, events, etc.)
        /// </summary>
        WorldInformationRequest = 1,

        /// <summary>
        /// Login server request for <see cref="CLogin.OnCheckPasswordResult"/>
        /// </summary>
        CheckPasswordRequest = 2,

        CheckDuplicateIdRequest = 3,
        CheckConnectionTest = 255,
    }
}