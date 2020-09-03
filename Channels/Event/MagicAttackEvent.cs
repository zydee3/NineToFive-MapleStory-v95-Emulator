using System;
using NineToFive.Constants;
using NineToFive.Event.Data;
using NineToFive.Net;

namespace NineToFive.Event {
    public class MagicAttackEvent : PacketEvent {
        
        private Attack _attack;
        
        public MagicAttackEvent(Client client) : base(client) { }
        
        public override bool OnProcess(Packet p) {
            _attack = new Attack(Client.User, p, AttackType.Magic);
            return true;
        }

        public override void OnHandle() {
            _attack.Complete(Client.User);
        }
    }
}