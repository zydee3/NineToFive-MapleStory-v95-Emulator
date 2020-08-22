namespace NineToFive.Event {
    public class CancelPartyWantedEvent : PacketEvent {
        public CancelPartyWantedEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return false;
        }
    }
}