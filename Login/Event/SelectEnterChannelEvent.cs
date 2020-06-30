using System;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SelectEnterChannelEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelectEnterChannelEvent));
        private byte _worldId, _channelId;
        private byte[] _address; // not sure

        public SelectEnterChannelEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Write(GetSelectWorldFailed(6));
        }

        public override bool OnProcess(Packet p) {
            byte b = p.ReadByte();
            if (b != 2) Log.Info($"unknown byte, unexpected value ${b}");
            _worldId = p.ReadByte();
            _channelId = p.ReadByte();
            _address = p.ReadBytes(4);
            if (_worldId >= Server.Worlds.Length) {
                Log.Warn($"invalid world specified : {_worldId}");
                return false;
            }

            if (_channelId >= Server.Worlds[_worldId].Channels.Length) {
                Log.Warn($"invalid channel specified {_channelId} for world {_worldId}");
                return false;
            }

            return true;
        }

        public override void OnHandle() {
            // the central server will relay this packet to the specified channel server which
            // in turn will return the amount of users connected. login -> central -> channel -> central -> login
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.ChannelUserLimitRequest);
            w.WriteByte(_worldId);
            w.WriteByte(_channelId);
            byte[] response = Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort);
            if (response != null) {
                Log.Info($"Updating user count for channel {_channelId} in world {_worldId}");
                Client.Channel.Snapshot.UserCount = BitConverter.ToInt32(response);
            }

            Client.SetWorld(_worldId);
            Client.SetChannel(_channelId);
            Client.Session.Write(GetSelectWorld());
        }

        /// <summary>
        /// <code>6,8,9 for    "Trouble logging in? Try logging in again from maplestory.nexon.net."</code>
        /// <code>2,3 for    "This is an ID that has been deleted or blocked from connection."</code>
        /// <code>4 for    "This is an incorrect password."</code>
        /// <code>5 for    "This is not a registered ID."</code>
        /// <code>7 for    "This is an ID that is already logged in, or the server in under inspection."</code>
        /// <code>10 for    "Could not be processed due to too many connection requests to the server."</code>
        /// <code>11 for    "Only those who are 20 years old or older can use this."</code>
        /// <code>13 for    "Unable to log-on as a master at IP."</code>
        /// <code>14 for    open_web_site(open_web_site(http://www.nexon.net)</code>
        /// <code>15 for    open_web_site(open_web_site(http://www.nexon.net)</code>
        /// <code>16,21 for    "Please verify your account via email in order to play the game."</code>
        /// <code>17 for    "You have either selected the wrong gateway, or you have yet to change your personal information."</code>
        /// <code>25 for    "You're logging in from outside of the service region."</code>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private byte[] GetSelectWorldFailed(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSelectWorldResult);
            p.WriteByte(a);
            return p.ToArray();
        }

        private byte[] GetSelectWorld() {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSelectWorldResult);
            p.WriteByte();                          // failure result
            p.WriteByte((byte) Client.Users.Count); // up to 15 characters
            foreach (User user in Client.Users) {
                user.CharacterStat.Encode(user, p);
                user.AvatarLook.Encode(user, p);
                p.WriteByte();
                if (p.WriteBool(ServerConstants.EnabledRanking)) {
                    p.WriteInt(); // nTotRank
                    p.WriteInt(); // nTotRankGap
                    p.WriteInt(); // nWorldRank
                    p.WriteInt(); // nworldRankGap
                }
            }

            // m_bLoginOpt
            // 0 for    CSoftKeyboardDlg::InitializeSecondaryPassword
            // 1 for    CSoftKeyboardDlg::GetResult
            // 2,3 for    no secondary password
            // if (ServerConstants.EnabledSecondaryPassword) {
                p.WriteByte((byte) (Client.SecondaryPassword == null ? 0 : 1));
            // } else {
                // p.WriteByte(2);
            // }

            p.WriteInt(Client.Users.Capacity - Client.Users.Count);
            p.WriteInt(); // m_nBuyCharCount
            return p.ToArray();
        }
    }
}