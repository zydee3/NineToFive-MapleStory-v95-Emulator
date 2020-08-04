using NineToFive.Net;

namespace NineToFive.Event {
    public class UpdatePassiveSkillDataEvent : PacketEvent {
        public UpdatePassiveSkillDataEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            return true;
        }

        public override void OnHandle() { }
    }
}