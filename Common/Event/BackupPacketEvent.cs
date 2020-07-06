using System;
using log4net;
using NineToFive.Net;

namespace NineToFive.Event {
    public class BackupPacketEvent : PacketEvent {
        public BackupPacketEvent(Client client) : base(client) { }
        private static readonly ILog Log = LogManager.GetLogger(typeof(BackupPacketEvent));
        public override bool OnProcess(Packet p) {
            Log.Info($"[BackupPacket] {p.ToArrayString(true)}");
            return false; // no reason to continue
        }

        public override void OnHandle() {
            
        }
    }
}