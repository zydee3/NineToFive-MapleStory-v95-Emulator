using System;
using System.Timers;
using NineToFive.Net;

namespace NineToFive.Event {
    public class KeepAliveEvent : PacketEvent {
        public KeepAliveEvent(Client client) : base(client) { }

        public override bool ShouldProcess() {
            return true;
        }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() {
            Client.PongTimestamp = DateTime.Now.TimeOfDay;
            if (Client.User?.IsDebugging == true) {
                Client.User.SendMessage($"{Client.PongTimestamp - Client.PingTimestamp}");
            }

            if (Client.PingTimer == null) {
                var timer = (Client.PingTimer = new Timer(1000 * 30));
                timer.Elapsed += DisposeIfNecessary;
                timer.Enabled = true;
                timer.AutoReset = true;
                timer.Start();
            }
        }

        private void DisposeIfNecessary(object sender, ElapsedEventArgs e) {
            Client.SendKeepAliveRequest();
            if (Client.Session.ShouldDispose()) {
                Client.Dispose();
            }
        }
    }
}