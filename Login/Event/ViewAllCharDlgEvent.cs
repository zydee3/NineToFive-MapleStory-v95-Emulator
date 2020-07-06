using NineToFive.Event;
using NineToFive.Net;

namespace NineToFive.Login.Event {
    public class ViewAllCharDlgEvent : PacketEvent {
        private bool _make;

        public ViewAllCharDlgEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _make = p.ReadByte() != 0;
            return true;
        }

        public override void OnHandle() {
            if (!_make) {
                Client.Session.Write(ViewAllCharEvent.GetViewAllCharFail(8));
            }
        }
    }
}