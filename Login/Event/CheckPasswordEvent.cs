using System;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    class CheckPasswordEvent : PacketEvent {
        private byte[] _machineId;
        private string _username, _password;

        public CheckPasswordEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            // send an error so the user doesn't get soft-locked
            Client.Session.Write(GetLoginFailed(6));
        }

        public override void OnHandle() {
            Client.Username = _username;
            Client.MachineId = _machineId;
            if (Client.TryLogin(_password)) {
                Interoperability.SendWorldInformationRequest();
                Client.Session.Write(GetLoginSuccess(Client));
            } else {
                Client.Session.Write(GetLoginFailed(4));
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
            return true;
        }

        internal static byte[] GetLoginSuccess(Client client) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnCheckPasswordResult);
            p.WriteByte();  // failure result see GetLoginFailed
            p.WriteByte(1); // success result
            p.WriteInt();   // unknown

            p.WriteInt(client.Id + 1);
            p.WriteByte(client.Gender);
            p.WriteByte();
            p.WriteShort();
            p.WriteByte();
            p.WriteString(client.Username);
            p.WriteByte();
            p.WriteByte();
            p.WriteLong();
            p.WriteLong();
            p.WriteInt();

            if (client.Gender != 10) {
                byte v26 = 1;
                p.WriteByte(v26);
                p.WriteByte(1);
                if (v26 == 0) {
                    p.WriteLong();
                }
            }

            return p.ToArray();
        }

        /// <summary>
        /// Rejects user login with a specified notice
        /// <para>6,8,9    for "Trouble logging in? Try logging in again from maplestory.nexon.net."</para>
        /// <para>2,3      for "This is an ID that has been deleted or blocked from the connection."</para>
        /// <para>4        for "This is an incorrect password."</para>
        /// <para>5        for "This is not a registered ID."</para>
        /// <para>7        for "This is an ID that is already logged in, or the server is under inspection."</para>
        /// <para>10       for "Could not be processed due to too many connection requests to the server."</para>
        /// <para>11       for "Only those who are 20 years old or odler can use this."</para>
        /// <para>13       for "Unable to log-on as a master at IP."</para>
        /// <para>15       for "We're still processing your request at this time, so you don't have access to this game for now."</para>
        /// <para>16,21    for "Please verify your account via email in order to play the game."</para>
        /// <para>14,17    for "You have either selected the wrong gateway, or you have yet to chagne your personal information."</para>
        /// <para>23       for CLicenseDlg::CLicenseDlg</para>
        /// <para>25       for "You're logging in from outside of the service region."</para>
        /// <para>27       for "Please download the full client to experience \r\nthe world of MapleStory. \r\nWould you like to download the full client\r\n from our website?"</para>
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