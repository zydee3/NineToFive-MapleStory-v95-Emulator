using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class EmotionChangeEvent : PacketEvent {
        private int _emotion;
        private int _duration;
        private bool _byItemOption;

        public EmotionChangeEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _emotion = p.ReadInt();
            _duration = p.ReadInt();
            _byItemOption = p.ReadBool();

            if (_emotion >= 1 && _emotion <= 7) {
                return _byItemOption == false;
            }

            return false;
        }

        public override void OnHandle() {
            var user = Client.User;
            user.Field.BroadcastPacketExclude(user, GetUserEmotionChanged(user.CharacterStat.Id, _emotion, _duration, _byItemOption));
        }

        private static byte[] GetUserEmotionChanged(uint characterId, int emotion, int duration, bool byItemOption) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserRemote.OnEmotion);
            w.WriteUInt(characterId);
            w.WriteInt(emotion);
            w.WriteInt(duration);
            w.WriteBool(byItemOption);
            return w.ToArray();
        }
    }
}