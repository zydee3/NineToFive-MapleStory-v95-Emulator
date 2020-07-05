using System;
using System.Net;
using MySql.Data.MySqlClient;
using NineToFive.Event;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Packets;
using NineToFive.SendOps;
using NineToFive.Util;

namespace NineToFive.Channels.Event {
    public class CharEnterGameEvent : PacketEvent {
        private uint _playerId;

        public CharEnterGameEvent(Client client) : base(client) { }

        public override void OnError(Exception e) { }

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
            Client.User.Field = Client.Channel.GetField(Client.User.CharacterStat.FieldId);
            Client.Session.Write(SetField(Client.User));
        }

        private static byte[] SetField(User user) {
            Field field = user.Field;

            using Packet w = new Packet();
            w.WriteShort((short) CStage.OnSetField);

            #region CClientOptMan::DecodeOpt

            w.WriteShort();

            #endregion

            w.WriteInt(Math.Abs(field.VRRight) - Math.Abs(field.VRLeft)); // nFieldWidth
            w.WriteInt(Math.Abs(field.VRBottom) - Math.Abs(field.VRTop)); // nFieldHeight
            w.WriteByte();                                                // unknown
            bool characterData = w.WriteBool(true);
            short notifierCheck = w.WriteShort();
            if (notifierCheck > 0) {
                w.WriteString();
                for (int i = 0; i < notifierCheck; i++) {
                    w.WriteString();
                }
            }

            if (characterData) {
                #region CalcDamage::SetSeed

                w.WriteInt();
                w.WriteInt();
                w.WriteInt();

                #endregion

                UserPackets.EncodeCharacterData(user, w, -1);
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