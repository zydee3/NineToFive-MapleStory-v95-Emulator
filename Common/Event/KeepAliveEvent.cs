using System;

namespace NineToFive.Event {
    public class KeepAliveEvent : PacketEvent {
        public KeepAliveEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return true;
        }

        public override void OnHandle() {
            Client.PingTimestamp = DateTime.Now.TimeOfDay;
        }
    }
}