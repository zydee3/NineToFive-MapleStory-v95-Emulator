using NineToFive.Event;
using NineToFive.Net;

namespace NineToFive.Login.Event {
    public class CWvsUiMenuEvent : PacketEvent {
        public CWvsUiMenuEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            // typically received when a UI component is closed
            p.ReadBool(); // MEMORY[0x98] != 0
            p.ReadBool(); // MEMORY[0x9C] != 0
            return false; // no reason to continue
        }

        public override void OnHandle() {
        }
    }
}