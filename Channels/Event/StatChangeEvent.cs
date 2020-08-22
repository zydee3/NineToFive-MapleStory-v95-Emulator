using NineToFive.Game.Entity.Meta;
using NineToFive.Net;

namespace NineToFive.Event {
    public class StatChangeEvent : PacketEvent {
        private uint _dwcharFlags;
        private int _hp, _mp;

        public StatChangeEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _dwcharFlags = p.ReadUInt();
            _hp = p.ReadShort();
            _mp = p.ReadShort();
            p.ReadByte();
            return (UserAbility) _dwcharFlags == (UserAbility.HP | UserAbility.MP);
        }

        public override void OnHandle() {
            var user = Client.User;
            user.CharacterStat.HP += _hp;
            user.CharacterStat.MP += _mp;
            user.CharacterStat.SendUpdate(user, _dwcharFlags);
        }
    }
}