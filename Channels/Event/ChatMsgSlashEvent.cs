using log4net;
using NineToFive.Net;

namespace NineToFive.Event {
    public class ChatMsgSlashEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatMsgSlashEvent));
        private string _msg;
        private byte _cmdAction;
        public ChatMsgSlashEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.Position = 0;
            short op = p.ReadShort();
            if (op == (int) ReceiveOperations.Field_LogChatMsgSlash) {
                _msg = p.ReadString();
                return true;
            }

            _cmdAction = p.ReadByte();
            return true;
        }

        public override void OnHandle() {
            if (_msg != null) {
                Log.Info($"command logged : {_msg}");
                return;
            }
        }
    }
}