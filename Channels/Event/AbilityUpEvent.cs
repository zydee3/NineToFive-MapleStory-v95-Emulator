using System;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Event {
    public class AbilityUpEvent : PacketEvent {
        private uint _dwcharFlag;

        public AbilityUpEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            byte[] b = p.ReadBytes(4);
            _dwcharFlag = BitConverter.ToUInt32(b, 0);
            if (Client.User.CharacterStat.AP < 1) return false;

            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            CharacterStat stat = user.CharacterStat;

            switch ((UserAbility) _dwcharFlag) {
                default:
                    Console.WriteLine($"unknown flag: {_dwcharFlag}");
                    return;
                case UserAbility.Str:
                    stat.Str += 1;
                    break;
                case UserAbility.Dex:
                    stat.Dex += 1;
                    break;
                case UserAbility.Int:
                    stat.Int += 1;
                    break;
                case UserAbility.Luk:
                    stat.Luk += 1;
                    break;
                case UserAbility.MaxHP:
                    stat.MaxHP += Randomizer.GetInt(8, 13);
                    break;
                case UserAbility.MaxMP: {
                    int advancement = (int) ((Math.Floor(stat.Job % 100d / 10d) + 1) * 10);
                    stat.MaxMP += (advancement) + (stat.Int / 10);
                    break;
                }
            }

            stat.AP -= 1;

            user.CharacterStat.SendUpdate(user, _dwcharFlag | (uint) UserAbility.AP);
        }
    }
}