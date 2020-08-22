namespace NineToFive.Event.Data {
    public class CheckOpBoardHasNewEvent : PacketEvent {
        public CheckOpBoardHasNewEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return false;
        }
    }
}