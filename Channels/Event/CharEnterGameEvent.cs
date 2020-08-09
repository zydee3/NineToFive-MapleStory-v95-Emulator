using System;
using System.Linq;
using System.Net;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Net.Interoperations.Event;
using NineToFive.Packets;
using NineToFive.Util;

namespace NineToFive.Event {
    public class CharEnterGameEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CharEnterGameEvent));
        private uint _playerId;

        public CharEnterGameEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return Client.LoginStatus == 0;
        }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Dispose();
        }

        public override bool OnProcess(Packet p) {
            _playerId = p.ReadUInt();
            // retrieve user data
            using DatabaseQuery qchar = Database.Table("characters");
            using MySqlDataReader rchar = qchar.Select().Where("character_id", "=", _playerId).ExecuteReader();
            if (!rchar.Read()) throw new InvalidOperationException("Failure to find character : " + _playerId);

            // get belonging account
            int accountId = rchar.GetInt32("account_id");
            using DatabaseQuery qacc = Database.Table("accounts");
            using MySqlDataReader racc = qacc.Select().Where("account_id", "=", accountId).ExecuteReader();
            if (!racc.Read()) throw new InvalidOperationException("Failure to find account : " + accountId);
            // check if IP address of the associated account belongs to the current session
            IPAddress lastKnownIp = IPAddress.Parse(racc.GetString("last_known_ip"));
            if (racc.GetByte("login_status") != 2 || !lastKnownIp.Equals(Client.Session.RemoteAddress)) {
                throw new InvalidOperationException("Possible remote hack");
            }

            Client.User = new User(rchar);
            Client.Username = racc.GetString("username");
            Log.Info($"{Client.User.CharacterStat.Username} is on channel {Client.Channel.Id}");
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            user.Client = Client;
            Client.Id = user.AccountId;
            Client.LoginStatus = 1;
            Client.SetChannel(Server.Worlds[World.ActiveWorld].Channels.First(ch => ch.Port == Client.Channel.Port).Id);
            Client.World.Users.AddOrUpdate(user.CharacterStat.Id, id => user, (id, u) => user);
            user.SetField(user.CharacterStat.FieldId);
            user.Client.Session.Write(CWvsPackets.GetStatChanged(user, 0));
            ClientAuthRequest.RequestClientUpdate(Client);
        }
    }
}