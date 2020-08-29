using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Net;

namespace NineToFive.Event.Data {
    public class Attack {
        public AttackType AttackType { get; }
        
        public int AttackSpeed { get; }
        public int AttackStance { get; }
        public int SkillId { get; }
        public int HitsPerMob { get; }
        public int MobsHit { get; }
        
        public byte FieldOffset { get; }
        public bool IsFacingRight { get; }
        
        internal Hit[] Hits { get; }
        
        public Attack(User user, Packet p, AttackType attackType) {
            AttackType = attackType;
            
            FieldOffset = p.ReadByte();
            
            p.ReadInt(); 
            p.ReadInt();
            
            int a4 = p.ReadByte();
            HitsPerMob = a4 & 0xF;
            MobsHit = a4 >> 4;

            if (MobsHit <= 0) {
                return;
            }

            p.ReadInt(); 
            p.ReadInt();
            
            SkillId = p.ReadInt(); 
            // check mob hit and hits per mob is valid
            
            p.ReadByte();
            p.ReadInt();  
            p.ReadInt();
            p.ReadInt();
            p.ReadInt();
            p.ReadByte();
            
            short a14 = p.ReadShort();
            IsFacingRight = a14 >> 15 == 0;
            AttackStance = a14 & 0x7FFF;
            
            p.ReadInt();
            p.ReadByte();
            
            AttackSpeed = p.ReadByte();
            
            p.ReadInt();
            p.ReadInt();

            Hits = new Hit[MobsHit];
            for (int i = 0; i < MobsHit; i++) {
                Hits[i] = new Hit(user, p, HitsPerMob);
            }
        }

        public async Task Complete() {
            foreach (Hit hit in Hits) hit.Complete();
        }
    }

    internal class Hit {
        private readonly User _user;
        private readonly bool _complete = true;
        private readonly int _damage;
        private readonly uint _mobId;
        
        
        public Hit(User user, Packet p, int hitsPerMob) {
            try {
                _user = user;
                _mobId = p.ReadUInt();
                p.ReadByte();
                p.ReadByte();
                p.ReadByte();
                p.ReadByte();
                
                p.ReadShort();
                p.ReadShort();
                
                p.ReadShort();
                p.ReadShort();

                p.ReadShort();

                for (int i = 0; i < hitsPerMob; i++) {
                    _damage += p.ReadInt();
                }

                p.ReadInt();
            } catch (Exception exception) {
                Console.WriteLine(exception.Message);
                _complete = false;
            }
        }

        public async Task Complete() {
            if (!_complete) return;
            Mob mob = _user.Field.LifePools[EntityType.Mob][_mobId] as Mob;
            mob?.Damage(_user, _damage);
        }
    }
}