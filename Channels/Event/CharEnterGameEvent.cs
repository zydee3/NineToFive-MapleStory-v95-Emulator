using System;
using System.Net;
using MySql.Data.MySqlClient;
using NineToFive.Event;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.IO;
using NineToFive.Packets;
using NineToFive.SendOps;
using NineToFive.Util;

namespace NineToFive.Channels.Event {
    public class CharEnterGameEvent : PacketEvent {
        private uint _playerId;

        public CharEnterGameEvent(Client client) : base(client) { }

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
            using MySqlDataReader racc = qacc.Select().Where("account_id", "=", rchar.GetInt32("account_id")).ExecuteReader();
            if (!racc.Read()) throw new InvalidOperationException("Failure to find account : " + accountId);
            // check if IP address of the associated account belongs to the current session
            IPAddress lastKnownIp = IPAddress.Parse(racc.GetString("last_known_ip"));
            if (!lastKnownIp.Equals(Client.Session.RemoteAddress)) {
                throw new InvalidOperationException("Possible remote hack");
            }

            Client.User = new User(rchar);
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            Client.Channel.GetField(user.CharacterStat.FieldId).AddLife(user);
            Client.Session.Write(SetField(user));
        }

        private static byte[] SetField(User user, bool characterData = true) {
            Field field = user.Field;

            using Packet w = new Packet();
            w.WriteShort((short) CStage.OnSetField);

            // CClientOptMan::DecodeOpt
            for (int i = 0; i < w.WriteShort(); i++) {
                w.WriteInt();
                w.WriteInt();
            }

            w.WriteInt(Math.Abs(field.VrRight) - Math.Abs(field.VrLeft)); // nFieldWidth
            w.WriteInt(Math.Abs(field.VrBottom) - Math.Abs(field.VrTop)); // nFieldHeight
            w.WriteByte(1);                                               // unknown
            w.WriteBool(characterData);
            short notifierCheck = w.WriteShort();
            if (notifierCheck > 0) {
                w.WriteString();
                for (int i = 0; i < notifierCheck; i++) {
                    w.WriteString();
                }
            }

            if (characterData) {
                // CalcDamage::SetSeed
                w.WriteInt(RNG.GetInt());
                w.WriteInt(RNG.GetInt());
                w.WriteInt(RNG.GetInt());

                UserPackets.EncodeCharacterData(user, w, -1);
                // CWvsContext::OnSetLogoutGiftConfig
                w.WriteInt();
                for (int i = 0; i < 3; i++) {
                    w.WriteInt();
                }
            } else {
                w.WriteBool(false); // CWvsContext::OnRevive
                w.WriteInt(user.CharacterStat.FieldId);
                w.WriteByte(user.CharacterStat.Portal);
                w.WriteInt(user.CharacterStat.HP);
                if (w.WriteBool(false)) {
                    w.WriteInt();
                    w.WriteInt();
                }
            }

            w.WriteLong(DateTime.Now.ToFileTime()); // paramFieldInit.ftServer

            return w.ToArray();
        }
    }
}