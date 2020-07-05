using System;
using System.Text.RegularExpressions;
using log4net;
using NineToFive.Constants;
using NineToFive.Event;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Login.Event {
    public class CheckDuplicateUsernameEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CheckDuplicateUsernameEvent));
        private string _username;
        private byte _result;

        public CheckDuplicateUsernameEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Write(GetCheckDuplicatedIdResult(_username, 3));
        }

        public override bool OnProcess(Packet p) {
            _username = p.ReadString();
            bool valid = Regex.Match(_username, "^[a-zA-Z0-9]{4,13}$").Success;
            if (!valid) {
                // straight up invalid, skip interoperations and querying
                _result = 2;
                return true;
            }

            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.CheckDuplicateIdRequest);
            w.WriteString(_username);
            _result = Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort)[0];
            return true;
        }

        public override void OnHandle() {
            Client.User = new User() {
                AccountId = Client.Id
            };

            Client.User.CharacterStat.Username = _username;
            Client.Session.Write(GetCheckDuplicatedIdResult(_username, _result));
        }

        /// <summary>
        /// the username check response packet
        /// <para>1    for "The name is currently being used."</para>
        /// <para>2    for "You cannot use this name."</para>
        /// <para>3+    for "Failed due to unknown reason."</para>
        /// </summary>
        /// <param name="username">username that is being tested</param>
        /// <param name="result">result of ability to use specified username</param>
        private static byte[] GetCheckDuplicatedIdResult(string username, byte result) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnCheckDuplicatedIDResult);
            p.WriteString(username);
            p.WriteByte(result);
            return p.ToArray();
        }
    }
}