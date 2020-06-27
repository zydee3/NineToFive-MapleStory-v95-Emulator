using System;
using NineToFive.IO;

namespace NineToFive.Event {
    public class BackupPacketEvent : PacketEvent {
        public BackupPacketEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            Console.WriteLine($"[BackupPacket] {p.ToArrayString(true)}");
            return false; // no reason to continue
        }

        public override void OnHandle() {
            
        }
    }
}