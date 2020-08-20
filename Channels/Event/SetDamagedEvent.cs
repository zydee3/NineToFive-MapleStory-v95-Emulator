using System;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;

namespace NineToFive.Event {
    public class SetDamagedEvent : PacketEvent {
        public SetDamagedEvent(Client client) : base(client) { }

        private int time;
        private int damage;
        private int mobTemplateId;
        private uint mobId;
        public override bool OnProcess(Packet p) {
            time = p.ReadInt();
            byte a2 = p.ReadByte();
            
            if (a2 == 255) { // COutPacket::Encode1(&v243, (a10 == dwObstacleData) - 3); // -3 will always be < 0
                p.ReadByte(); // ?
                damage = p.ReadInt();
                mobTemplateId = p.ReadInt();
                mobId = p.ReadByte();
                //p.ReadInt(); // ?
                //p.ReadInt()  // ?0
            }
            
            return true;
        }

        public override void OnHandle() {
            if (time <= 0 || damage <= 0 || mobTemplateId < 0 || mobId <= 0) {
                return;
            }

            User user = Client.User;
            Mob mob = user.Field.LifePools[EntityType.Mob][mobId] as Mob;
            if (mob == null) {
                return;
            }

            user.CharacterStat.HP -= damage;
            user.CharacterStat.SendUpdate(user, (uint)UserAbility.HP);
        }
    }
}
