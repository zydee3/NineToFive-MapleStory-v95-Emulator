using System.Numerics;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.Wz;

namespace NineToFive.Event {
    public class UserSkillUseEvent : PacketEvent {
        private int _skillId;
        private byte _skillLevel;
        private Vector2 _origin;

        public UserSkillUseEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _skillId = p.ReadInt();
            _skillLevel = p.ReadByte();
            // if ( is_antirepeat_buff_skill )
            //     _origin = new Vector2(p.ReadShort(), p.ReadShort());
            if (_skillId == 4121006) {
                // Shadow Stars
                p.ReadInt();
            }

            Client.User.Skills.TryGetValue(_skillId, out var skillRecord);
            return skillRecord?.Level == _skillLevel;
        }

        public override void OnHandle() {
            User user = Client.User;
            var skillRecord = user.Skills[_skillId];
            user.SendMessage($"Skill: {_skillId}, Level: {skillRecord.Level}, Received: {_skillLevel}");

            SkillWz.Skills.TryGetValue(skillRecord.Id, out var skill);
            if (skill != null) {
                Client.Session.Write(CWvsPackets.GetTemporaryStatSet(skill, skillRecord));
            }

            user.CharacterStat.SendUpdate(user, 0);
        }
    }
}