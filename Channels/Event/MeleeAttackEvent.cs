using NineToFive.Constants;
using NineToFive.Event.Data;
using NineToFive.Net;

namespace NineToFive.Event {
    public class MeleeAttackEvent : PacketEvent {
        private Attack _attack;
        
        public MeleeAttackEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _attack = new Attack(Client.User, p, AttackType.Melee);
            return true;
        }

        public override void OnHandle() {
            _attack.Complete();
        }
    }
}