using System;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;

namespace NineToFive.Event {
    public class SetDamagedEvent : PacketEvent {
        public SetDamagedEvent(Client client) : base(client) { }

        private int _time;
        private int _damage;
        private int _mobTemplateId;
        private uint _mobId;
        public override bool OnProcess(Packet p) {
            _time = p.ReadInt();
            
            if (p.ReadByte() == 255) { // COutPacket::Encode1(&v243, (a10 == dwObstacleData) - 3); // -3 will always be < 0
                p.ReadByte(); // ?
                _damage = p.ReadInt();
                _mobTemplateId = p.ReadInt();
                _mobId = p.ReadByte();
                //p.ReadInt(); // ?
                //p.ReadInt()  // ?0
            }
            
            return true;
        }

        public override void OnHandle() {
            if (_time <= 0 || _damage <= 0 || _mobTemplateId <= 0 || _mobId < 0) {
                return;
            }

            User user = Client.User;
            Mob mob = user.Field.LifePools[EntityType.Mob][_mobId] as Mob;
            if (mob == null) {
                return;
            }

            user.CharacterStat.HP -= _damage;
            user.CharacterStat.SendUpdate(user, (uint)UserAbility.HP);
        }
    }
}
