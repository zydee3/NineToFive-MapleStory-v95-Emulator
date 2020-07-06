using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class ChatMsgEvent : PacketEvent {
        private string _msg;
        private bool _shout;

        public ChatMsgEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _msg = p.ReadString();
            _shout = p.ReadBool();
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;

            using Packet w = new Packet();
            w.WriteShort((short) CUserLocal.OnChatMsg);
            w.WriteShort(); // msg type
            w.WriteString($"{user.CharacterStat.Username} : {_msg}");
            user.Client.Session.Write(w.ToArray());
            user.Field.Broadcast(GetUserMsg(user, _msg, _shout));
        }

        private static byte[] GetUserMsg(User user, string msg, bool shout) {
            using Packet w = new Packet();
            w.WriteShort((short) CUser.OnChat_Send);
            w.WriteUInt(user.CharacterStat.Id);
            w.WriteByte();
            w.WriteString(msg);
            w.WriteBool(shout);
            return w.ToArray();
        }
    }
}