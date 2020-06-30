using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.IO;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SelectEnterChannelEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelectEnterChannelEvent));
        private byte _worldId, _channelId;
        private byte[] _address;

        public SelectEnterChannelEvent(Client client) : base(client) { }

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
            Client.SetWorld(_worldId);
            Client.SetChannel(_channelId);

            Client.Session.Write(GetSelectWorldResult());
        }

        private byte[] GetSelectWorldResult() {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSelectWorldResult);
            p.WriteByte();                          // failure result
            p.WriteByte((byte) Client.Users.Count); // up to 15 characters
            foreach (User user in Client.Users) {
                user.CharacterStat.Encode(user, p);
                user.AvatarLook.Encode(user, p);
                p.WriteByte();
                p.WriteBool(ServerConstants.EnabledRanking);
                if (ServerConstants.EnabledRanking) {
                    p.WriteInt(); // nTotRank
                    p.WriteInt(); // nTotRankGap
                    p.WriteInt(); // nWorldRank
                    p.WriteInt(); // nworldRankGap
                }
            }

            // 0 for    CSoftKeyboardDlg::InitializeSecondaryPassword
            // 1 for    CSoftKeyboardDlg::GetResult
            // 2,3 for    no secondary password
            p.WriteByte(Client.LoginOption); // m_bLoginOpt
            p.WriteInt(Client.Users.Capacity - Client.Users.Count);
            p.WriteInt(); // m_nBuyCharCount
            return p.ToArray();
        }
    }
}