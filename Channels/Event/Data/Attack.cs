using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Net;

namespace NineToFive.Event.Data {
    public class Attack {
        public AttackType attackType { get; set; }
        public byte type { get; set; }
        public byte targeted { get; set; }
        public int skillId { get; set; }
        public int hitsPerMob { get; set; }
        public int mobsHit { get; set; }

        internal Hit[] hits { get; set; }
        
        public Attack(User user, Packet p, AttackType attackType) {
            this.attackType = attackType;
            
            Console.WriteLine(p.ToArrayString(true));
            
            type = p.ReadByte();
            int a2 = p.ReadInt();
            int a3 = p.ReadInt();
            
            targeted = p.ReadByte();
            hitsPerMob = targeted & 0xF;
            mobsHit = targeted >> 4;

            if (mobsHit == 0) {
                //return;
            }

            Console.WriteLine($"type={type} \na2={a2} \na3={a3} \ntargeted={targeted} \nhitsPerMob={hitsPerMob}");
            
            int a4 = p.ReadInt();
            int a5 = p.ReadInt();
            skillId = p.ReadInt();
            byte a6 = p.ReadByte();
            
            int a7 = p.ReadInt();
            int a8 = p.ReadInt();
            int a9 = p.ReadInt();
            int a10 = p.ReadInt();

            byte a11 = p.ReadByte();
            short a12 = p.ReadShort();
            int a13 = p.ReadInt();
            byte a14 = p.ReadByte();
            byte a15 = p.ReadByte();
            int a16 = p.ReadInt();
            int a17 = p.ReadInt();
            int a18 = p.ReadInt();

            hits = new Hit[mobsHit];
            for (int i = 0; i < mobsHit; i++) {
                hits[i] = new Hit(user, p);
            }
            
            Console.WriteLine($"a4={a4} \na5={a5} \nskill={skillId} \na6={a6} \na7={a7} \na8={a8} \na9={a9} \na10={a10} \na11={a11} \na12={a12} \na13={a13} \na14={a14} \na15={a15} \na16={a16} \na17={a17} \na18={a18} \nremaining={p.ReadRemaining()}\n\n");
        }

        public async Task Complete() {
            await Task.WhenAll(hits.Select(hit => hit.Complete()));
        }
    }

    internal class Hit {
        private readonly User _user;
        
        private int _damage;
        private uint _mobId;
        
        public Hit(User user, Packet p) {
            _user = user;
        }

        public async Task Complete() {
            Mob mob = _user.Field.LifePools[EntityType.Mob][_mobId] as Mob;
            if (mob == null) {
                return;
            }

            await mob.Damage(_user, _damage);
        }
    }
}