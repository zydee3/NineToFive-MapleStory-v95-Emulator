using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive {
    public class Client : IPacketSerializer<Client> {
        public readonly ServerListener ServerHandler;
        public readonly ClientSession Session;
        public readonly List<User> Users = new List<User>(15);
        private byte _worldId, _channelId;

        public Client(ServerListener server, TcpClient socket) {
            ServerHandler = server;
            Session = new ClientSession(this, socket);
            Gender = 10;
        }

        public void Encode(Client t, Packet p) {
            p.WriteUInt(t.Id);
            p.WriteString(t.Username);
            p.WriteByte(t.Gender);
            if (p.WriteBool(t.MachineId != null)) {
                p.WriteBytes(t.MachineId);
            }

            if (p.WriteBool(t.SecondaryPassword != null)) {
                p.WriteString(SecondaryPassword);
            }

            p.WriteBytes(LastKnownIp.GetAddressBytes());
        }

        public void Decode(Client t, Packet p) {
            t.Id = p.ReadUInt();
            t.Username = p.ReadString();
            t.Gender = p.ReadByte();
            if (p.ReadBool()) {
                t.MachineId = p.ReadBytes(16);
            }

            if (p.ReadBool()) {
                t.SecondaryPassword = p.ReadString();
            }

            t.LastKnownIp = new IPAddress(p.ReadBytes(4));
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public byte Gender { get; set; }
        public User User { get; set; }
        public byte[] MachineId { get; set; }
        public string SecondaryPassword { get; set; }
        public IPAddress LastKnownIp { get; set; }

        public World World => Server.Worlds[_worldId];

        public Channel Channel => World.Channels[_channelId];

        public void SetWorld(byte worldId) {
            _worldId = worldId;
        }

        public void SetChannel(byte channelId) {
            _channelId = channelId;
        }
    }
}