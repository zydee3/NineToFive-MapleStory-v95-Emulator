using System;
using NineToFive.Event.Data;
using NineToFive.Net;

namespace NineToFive.Event {
    public class BodyAttackEvent : PacketEvent {
        
        private Attack _attack;
        
        public BodyAttackEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            return true;
        }

        public override void OnHandle() {
            
        }
    }
}