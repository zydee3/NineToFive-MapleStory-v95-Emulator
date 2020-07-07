using System;
using System.Net;
using NineToFive.Event;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Login.Event {
    class CheckPasswordEvent : PacketEvent {
        private byte[] _machineId;
        private string _username, _password;

        public CheckPasswordEvent(Client client) : base(client) { }

        public byte GetAuthRequest() {
            if (_username.Length < 4 || _password.Length < 4) {
                return 5;
            }

            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.CheckPasswordRequest);
            w.WriteString(_username);
            w.WriteString(_password);
            w.WriteBytes(_machineId);

            using Packet r = new Packet(Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort));
            if (r.Size == 0) return 6;
            byte result = r.ReadByte();
            // un-successful login
            if (result != 1) return result;

            Client.Decode(Client, r);
            return result;
        }

        public override void OnError(Exception e) {
            base.OnError(e);
            // send an error so the user doesn't get soft-locked
            Client.Session.Write(GetLoginFailed(6));
        }

        public override void OnHandle() {
            byte loginResult = GetAuthRequest();
            if (loginResult == 1) {
                Client.Session.Write(GetLoginSuccess(Client));
            } else {
                Client.Session.Write(GetLoginFailed(loginResult));
            }
        }

        public override bool OnProcess(Packet packet) {
            _password = packet.ReadString();
            _username = packet.ReadString();
            _machineId = packet.ReadBytes(16);
            packet.ReadInt();  // CSystemInfo::GetGameRoomClient
            packet.ReadByte(); // MEMORY[0x38]
            packet.ReadByte(); // 0
            packet.ReadByte(); // 0
            packet.ReadInt();  // partnerCode
            if (!Interoperability.TestConnection(IPAddress.Parse(ServerConstants.CentralServer), ServerConstants.InterCentralPort)) {
                Client.Session.Write(GetLoginFailed(6));
                return false;
            }

            return true;
        }

        internal static byte[] GetLoginSuccess(Client client) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnCheckPasswordResult);
            p.WriteByte();  // failure result see GetLoginFailed
            p.WriteByte(1); // success result
            p.WriteInt();   // unknown

            p.WriteUInt(client.Id);
            p.WriteByte(client.Gender);
            // nGradeCode, nPurchaseExp, (bManager, bTester, bSubTester)
            p.WriteByte();
            p.WriteShort(client.GradeCode);
            p.WriteByte();

            p.WriteString(client.Username);
            p.WriteByte(); // nVIPGrade
            p.WriteByte(); // nPurchaseExp
            p.WriteLong(); // dtChatUnblockDate
            p.WriteLong(); // 
            p.WriteInt();  // 

            if (client.Gender != 10) {
                p.WriteByte(2);
                p.WriteByte(2);
                p.WriteLong();
            }

            return p.ToArray();
        }

        /// <summary>
        /// Rejects user login with a specified notice
        /// <code>6,8,9    for "Trouble logging in? Try logging in again from maplestory.nexon.net."</code>
        /// <code>2,3      for "This is an ID that has been deleted or blocked from the connection."</code>
        /// <code>4        for "This is an incorrect password."</code>
        /// <code>5        for "This is not a registered ID."</code>
        /// <code>7        for "This is an ID that is already logged in, or the server is under inspection."</code>
        /// <code>10       for "Could not be processed due to too many connection requests to the server."</code>
        /// <code>11       for "Only those who are 20 years old or odler can use this."</code>
        /// <code>13       for "Unable to log-on as a master at IP."</code>
        /// <code>15       for "We're still processing your request at this time, so you don't have access to this game for now."</code>
        /// <code>16,21    for "Please verify your account via email in order to play the game."</code>
        /// <code>14,17    for "You have either selected the wrong gateway, or you have yet to change your personal information."</code>
        /// <code>23       for CLicenseDlg::CLicenseDlg</code>
        /// <code>25       for "You're logging in from outside of the service region."</code>
        /// <code>27       for "Please download the full client to experience \r\nthe world of MapleStory. \r\nWould you like to download the full client\r\n from our website?"</code>
        /// </summary>
        /// <param name="a">Represents a message popup image in the directory: <code>UI.wz/Login.img/Notice/text</code></param>
        private static byte[] GetLoginFailed(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnCheckPasswordResult);
            p.WriteByte(a); // failure result
            // 0,1  for success
            // 2,3  for "open_web_site(http://passport.nexon.net/?PART=/MyMaple/Verifycode)"
            // anything else for CLoginUtilDlg::Error
            p.WriteByte(4);
            p.WriteInt(); // unknown
            if (a == 2) {
                p.WriteByte(1); // block reason
                p.WriteLong();  // date probably
            }

            return p.ToArray();
        }
    }
}