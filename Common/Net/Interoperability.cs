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

namespace NineToFive.Net {
    public class Interoperability {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Interoperability));
        private static readonly SimpleCrypto SimpleCrypto = new SimpleCrypto();

        public Interoperability() { }

        #region Server methods

        public delegate void OnInteroperationReceived(TcpClient c, Packet p);

        /// <summary>
        /// Create a tcp listening socket on the <see cref="ServerConstants.InternalPort"/>
        /// <para>Immediately accepts connections asynchronously</para>
        /// </summary>
        public static void ServerCreate(int port, OnInteroperationReceived handler) {
            void StartSocket() {
                TcpListener server = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
                server.Start();
                while (true) ServerAcceptClients(server, ref handler);
            }

            new Thread(StartSocket).Start();
        }

        /// <summary>
        /// Handles the incoming connection
        /// </summary>
        private static void ServerAcceptClients(TcpListener server, ref OnInteroperationReceived handler) {
            try {
                using TcpClient client = server.AcceptTcpClient();
                // the client may request data at any time, so be ready!
                byte[] buffer = new byte[4];
                using NetworkStream stream = client.GetStream();
                stream.Read(buffer, 0, buffer.Length);
                buffer = new byte[BitConverter.ToInt32(buffer)];
                stream.Read(buffer, 0, buffer.Length);
                using Packet p = new Packet(buffer);
                handler(client, p);
            } catch (Exception e) {
                Log.Error("Error while processing", e);
            }
        }

        #endregion

        /// <summary>
        /// Sends a request for consent to host the specified channels
        /// </summary>
        /// <param name="port">interoperability server to ask</param>
        /// <param name="address">remote address of the server requesting to host</param>
        /// <param name="worldId">id of world which the channels belong to</param>
        /// <param name="min">lower bound channel</param>
        /// <param name="max">upper bound channel</param>
        /// <returns></returns>
        public static bool SendChannelHostPermission(int port, byte[] address, byte worldId, byte min, byte max) {
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperations.ChannelHostPermission);
            w.WriteBytes(address);
            w.WriteByte(worldId);
            w.WriteByte(min);
            w.WriteByte(max);
            using Packet r = SendPacket(w.ToArray(), port);
            return r?.ReadBool() == true;
        }

        public static void SendWorldInformationRequest() {
            foreach (World world in Server.Worlds) {
                // obtain channel servers group by HostAddress
                var servers =
                    from ch in world.Channels
                    group ch by new {ch.Port, ch.Snapshot.HostAddress}
                    into results
                    select results;

                foreach (var server in servers) {
                    using Packet w = new Packet();
                    w.WriteByte((byte) Interoperations.WorldInformationRequest);
                    w.WriteByte(world.Id); // get user count for each server for this world 
                    using Packet r = SendPacket(w.ToArray(), ServerConstants.InterChannelPort, server.Key.HostAddress);
                    if (r == null) continue;
                    byte channelId;
                    do {
                        channelId = r.ReadByte();
                        if (channelId == 255) break;
                        world.Channels[channelId].Snapshot.UserCount = r.ReadInt();
                    } while (channelId != 255);
                }
            }
        }

        public static bool TestConnection(string address, int port) {
            try {
                using TcpClient client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                stream.WriteByte(255);
                return true;
            } catch {
                return false;
            }
        }

        private static Packet SendPacket(byte[] buffer, int port, string address = "127.0.0.1") {
            try {
                using TcpClient client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                byte[] request = SimpleCrypto.Encrypt(buffer);
                stream.Write(request, 0, request.Length);

                byte[] response = new byte[4];
                stream.Read(response, 0, response.Length);
                response = new byte[BitConverter.ToInt32(response)];
                stream.Read(response);
                return new Packet(response);
            } catch {
                // connection failed or something
                return null;
            }
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
        CheckConnectionTest = 255,
    }
}