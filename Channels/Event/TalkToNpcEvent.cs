using NineToFive.Net;

namespace NineToFive.Event {
    public class TalkToNpcEvent : PacketEvent {
        public TalkToNpcEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() { }
    }
}