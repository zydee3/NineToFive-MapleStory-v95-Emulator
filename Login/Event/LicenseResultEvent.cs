using NineToFive;
using NineToFive.Event;
using NineToFive.IO;

namespace NineToFive.Event {
    class LicenseResultEvent : PacketEvent {

        bool accept;

        public LicenseResultEvent(Client client) : base(client) {
        }

        public override void OnHandle() {
            if (accept) {
                Client.Session.Write(CheckPasswordEvent.GetLoginSuccess(Client));
            }
        }

        public override bool OnProcess(Packet p) {
            accept = p.ReadBool();
            return true;
        }
    }
}
