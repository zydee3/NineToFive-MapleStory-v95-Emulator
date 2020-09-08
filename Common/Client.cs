using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Net.Interoperations.Event;
using NineToFive.Util;

namespace NineToFive {
    public class Client : IPacketSerializer, IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));
        public readonly List<User> Users = new List<User>(15);
        public ClientSession Session;
        private byte _worldId, _channelId;

        public Client() {
            Gender = 10;
        }

        public Client(ClientSession session) : this() {
            Session = session;
        }

        public void SendKeepAliveRequest() {
            PingTimestamp = DateTime.Now.TimeOfDay;
            Session.Write(new byte[] {17, 0});
        }

        public void Dispose() {
            PingTimer?.Stop();
            PingTimer?.Dispose(); // WHICH DO I USE
            PingTimer = null;

            Users.Clear();
            try {
                User?.Dispose();

                // reset if client is logged-in, NOT during socket migration
                if (LoginStatus == 1) {
                    LoginStatus = 0;
                    ClientAuthRequest.RequestClientUpdate(this);
                    Log.Info($"'{Username}' : {User?.CharacterStat.Username} disconnected");
                }
            } catch (Exception e) {
                Log.Error($"Failure to disconnect '{Username}'", e);
            }
            Session = null;
        }

        public void Encode(Packet p) {
            p.WriteUInt(Id);
            p.WriteString(Username);
            p.WriteString(Password);
            p.WriteByte(Gender);
            p.WriteByte(GradeCode);
            p.WriteByte(LoginStatus);
            if (p.WriteBool(MachineId != null)) {
                p.WriteBytes(MachineId);
            }

            if (p.WriteBool(SecondaryPassword != null)) {
                p.WriteString(SecondaryPassword);
            }

            if (p.WriteBool(LastKnownIp != null)) {
                p.WriteBytes(LastKnownIp?.GetAddressBytes());
            }
        }

        public void Decode(Packet p) {
            Id = p.ReadUInt();
            Username = p.ReadString();
            Password = p.ReadString();
            Gender = p.ReadByte();
            GradeCode = p.ReadByte();
            LoginStatus = p.ReadByte();
            if (p.ReadBool()) {
                MachineId = p.ReadBytes(16);
            }

            if (p.ReadBool()) {
                SecondaryPassword = p.ReadString();
            }

            if (p.ReadBool()) {
                LastKnownIp = new IPAddress(p.ReadBytes(4));
            }
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte Gender { get; set; }
        public byte GradeCode { get; set; }

        /// <summary>
        /// <code>0    logged-out</code>
        /// <code>1    logged-in</code>
        /// <code>2    socket migrate</code>
        /// </summary>
        public byte LoginStatus { get; set; }

        public TimeSpan PingTimestamp { get; set; }
        public TimeSpan PongTimestamp { get; set; }
        public Timer PingTimer { get; set; }
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
            using MySqlDataReader r = q.Select()
                .Where("account_id", "=", Id,
                    "world", "=", _worldId).ExecuteReader();
            while (r.Read()) {
                Users.Add(new User(r));
            }
        }
    }
}