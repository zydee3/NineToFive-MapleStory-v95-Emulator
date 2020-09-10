using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.Resources;
using NineToFive.SendOps;
using NineToFive.Wz;

namespace NineToFive.Event {
    public class UserSkillCancelEvent : PacketEvent {
        private int _skillId;

        public UserSkillCancelEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _skillId = p.ReadInt();
            return true;
        }

        public override void OnHandle() {
            var user = Client.User;
            if (!user.Skills.TryGetValue(_skillId, out var record)) return;
            record.Proc = false;
            // user.Field.BroadcastPacketExclude(user, GetUserSkillCancel(user.CharacterStat.Id, _skillId));
            WzCache.Skills.TryGetValue(record.Id, out var skill);
            Client.Session.Write(CWvsPackets.GetTemporaryStatReset(skill));
        }

        private static byte[] GetUserSkillCancel(uint playerId, int skillId) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserRemote.OnSkillCancel);
            w.WriteUInt(playerId);
            w.WriteInt(skillId);
            return w.ToArray();
        }
    }
}