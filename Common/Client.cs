using System;
using System.Collections.Generic;
using System.Net;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive {
    public class Client : IPacketSerializer<Client>, IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));
        public readonly ClientSession Session;
        public readonly List<User> Users = new List<User>(15);
        private byte _worldId, _channelId;

        public Client() {
            Gender = 10;
        }

        public Client(ClientSession session) : this() {
            Session = session;
        }

        public void Dispose() {
            Users.Clear();
            if (Username == null) return;
            try {
                User?.Dispose();
                Log.Info($"'{Username}' disconnected");
            } catch (Exception e) {
                Log.Error($"Failure to disconnect '{Username}'", e);
            }
        }

        public void Encode(Client t, Packet p) {
            p.WriteUInt(t.Id);
            p.WriteString(t.Username);
            p.WriteString(t.Password);
            p.WriteByte(t.Gender);
            p.WriteByte(t.GradeCode);
            if (p.WriteBool(t.MachineId != null)) {
                p.WriteBytes(t.MachineId);
            }

            if (p.WriteBool(t.SecondaryPassword != null)) {
                p.WriteString(SecondaryPassword);
            }

            if (p.WriteBool(LastKnownIp != null)) {
                p.WriteBytes(LastKnownIp?.GetAddressBytes());
            }
        }

        public void Decode(Client t, Packet p) {
            t.Id = p.ReadUInt();
            t.Username = p.ReadString();
            t.Password = p.ReadString();
            t.Gender = p.ReadByte();
            t.GradeCode = p.ReadByte();
            if (p.ReadBool()) {
                t.MachineId = p.ReadBytes(16);
            }

            if (p.ReadBool()) {
                t.SecondaryPassword = p.ReadString();
            }

            if (p.ReadBool()) {
                t.LastKnownIp = new IPAddress(p.ReadBytes(4));
            }
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte Gender { get; set; }
        public byte GradeCode { get; set; }
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

        public void LoadCharacters() {
            using DatabaseQuery q = Database.Table("characters");
            using MySqlDataReader r = q.Select().Where("account_id", "=", Id).ExecuteReader();
            while (r.Read()) {
                Users.Add(new User(r));
            }
        }
    }
}