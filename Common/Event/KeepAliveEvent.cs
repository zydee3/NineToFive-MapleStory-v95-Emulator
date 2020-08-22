using System;
using System.Timers;

namespace NineToFive.Event {
    public class KeepAliveEvent : PacketEvent {
        public KeepAliveEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return true;
        }

        public override void OnHandle() {
            // response
            Client.PingTimestamp = DateTime.Now.TimeOfDay;

            if (Client.PingTimer == null) {
                var timer = (Client.PingTimer = new Timer(1000 * 40));
                timer.Elapsed += DisposeIfNecessary;
                timer.Enabled = true;
                timer.AutoReset = true;
                timer.Start();
            }
        }

        private void DisposeIfNecessary(object sender, ElapsedEventArgs e) {
            Client.Session.DisposeIfNecessary();
        }
    }
}