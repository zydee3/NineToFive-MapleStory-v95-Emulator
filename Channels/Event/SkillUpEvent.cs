using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SkillUpEvent : PacketEvent {
        private int _skillId;

        public SkillUpEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _skillId = p.ReadInt();
            return Client.User.CharacterStat.SP > 0;
        }

        public override void OnHandle() {
            User user = Client.User;

            // var skill = SkillWz.GetSkills().First(s => s.Key == _skillId).Value;
            Client.User.Skills.TryGetValue(_skillId, out SkillRecord record);
            if (record == null) {
                if (!JobConstants.CheckLineage((short) (_skillId / 10000), user.CharacterStat.Job)) return; // shouldn't happen unless they're packet editing.
                record = new SkillRecord(_skillId, 1);
                Client.User.Skills[_skillId] = record;
            } else record.Level += 1;

            user.CharacterStat.SP -= 1;

            Client.Session.Write(GetChangeSkillRecord(record));
            Client.User.CharacterStat.SendUpdate((uint) UserAbility.SP);
        }

        public static byte[] GetChangeSkillRecord(SkillRecord record) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnChangeSkillRecordResult);
            w.WriteBool(true); // get_update_time
            w.WriteShort(1);   // count

            w.WriteInt(record.Id);
            w.WriteInt(record.Level);
            w.WriteInt(record.MasterLevel);
            w.WriteLong(record.Expiration);

            w.WriteByte();
            return w.ToArray();
        }
    }
}