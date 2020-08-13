using NineToFive.Net;

namespace NineToFive.Event.Data {
    public class GenerateMovePathEvent : PacketEvent{
        public GenerateMovePathEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() {
            
        }
    }
}