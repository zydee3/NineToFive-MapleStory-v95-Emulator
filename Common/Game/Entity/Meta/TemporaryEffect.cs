using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
using NineToFive.Net;
using NineToFive.Resources;
using NineToFive.SendOps;
using NineToFive.Wz;

namespace NineToFive.Game.Entity.Meta {
    public class TemporaryEffect : IDisposable {
        
        private uint buffFlag;
        private uint statFlag;
        
        private readonly int _templateId;
        private readonly int _optional;
        private readonly bool _isSkill;

        private List<Task> Tasks;
        
        public TemporaryEffect(int templateId, bool isSkill, int optional = 0) {
            _templateId = templateId;
            _optional = optional;
            _isSkill = isSkill;
        }

        public async Task Apply(User user) {
            var stats = user.CharacterStat;

            if (_isSkill) {
                if (!WzCache.Skills.TryGetValue(_templateId, out Skill skill)) return;
            } else {
                ItemSlotBundleData data = ItemWz.GetItemData(_templateId);
                if (data == null) return;

                await Task.WhenAll(new List<Task> {
                    DoHpMpChange(stats, (int) (stats.MaxHP * (data.HpR * 1.0 / 100)) + data.Hp, (int) (stats.MaxMP * (data.MpR * 1.0 / 100)) + data.Mp),
                    DoPrimaryStatChange(stats, data.Str, data.Dex, data.Int, data.Luk),
                    DoSecondaryStatChange(user.Client, data),
                });
            }

            stats.SendUpdate(statFlag);
        }

        private async Task DoHpMpChange(CharacterStat stats, int hpGain, int mpGain) {
            if (hpGain > 0) {
                stats.HP += hpGain;
                statFlag |= (uint) UserAbility.HP;
            } 
                
            if (mpGain > 0) {
                stats.MP += mpGain;
                statFlag |= (uint) UserAbility.MP;
            }
        }
        
        private async Task DoPrimaryStatChange(CharacterStat stats, int strength, int dexterity, int intelligence, int luck) {
            if (strength > 0) {
                stats.Str += (short) strength;
                statFlag |= (uint) UserAbility.Str;
            }

            if (dexterity > 0) {
                stats.Dex += (short) dexterity;
                statFlag |= (uint) UserAbility.Dex;
            }

            if (intelligence > 0) {
                stats.Int += (short) intelligence;
                statFlag |= (uint) UserAbility.Int;
            }

            if (luck > 0) {
                stats.Luk += (short) luck;
                statFlag |= (uint) UserAbility.Luk;
            }
        }
        
        private async Task DoSecondaryStatChange(Client client, ItemSlotBundleData data) {
            var buffs = data.GetBuffValues();
            if (buffs.Count == 0) return;
            
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnTemporaryStatSet);

            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt((int) data.BitMask);

            foreach (var value in buffs) {
                w.WriteShort(value);
                w.WriteInt(-data.TemplateId);
                w.WriteInt(data.Time * 1000);
            }

            w.WriteByte();
            w.WriteByte();

            w.WriteShort(1);
            // SecondaryStat::IsMovementAffectingStat
            w.WriteByte(1);
            client.Session.Write(w.ToArray());
        }

        public void Dispose() {}
    }
}