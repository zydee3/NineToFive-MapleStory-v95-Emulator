namespace NineToFive.Event {
    public class ResetNLCPQEvent : PacketEvent {
        public ResetNLCPQEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return false;
        }
    }
}