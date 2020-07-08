using NineToFive.Net;

namespace NineToFive.Event {
    class LicenseResultEvent : PacketEvent {

        private bool _accept;

        public LicenseResultEvent(Client client) : base(client) {
        }

        public override void OnHandle() {
            if (_accept) {
                Client.Session.Write(CheckPasswordEvent.GetLoginSuccess(Client));
            }
        }

        public override bool OnProcess(Packet p) {
            _accept = p.ReadBool();
            return true;
        }
    }
}
