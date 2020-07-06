using NineToFive.Event;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Login.Event {
    /// <summary>
    /// <para>CLogin::SendCancelGenderPacket</para>
    /// <para>CLogin::SendSetGenderPacket</para>
    /// </summary>
    class SetGenderEvent : PacketEvent {
        private bool _success;
        private byte _gender;

        public SetGenderEvent(Client client) : base(client) { }

        public override void OnHandle() {
            if (_success) {
                Client.Gender = _gender;
                using Packet w = new Packet();
                w.WriteByte((byte) Interoperation.ClientGenderUpdateRequest);
                w.WriteString(Client.Username);
                w.WriteByte(_gender);
                Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort);
            }

            Client.Session.Write(GetSetAccountResult(_gender, _success));
        }

        public override bool OnProcess(Packet p) {
            _success = p.ReadBool();
            if (_success) _gender = p.ReadByte();
            return _gender == 0 || _gender == 1;
        }

        private static byte[] GetSetAccountResult(byte gender, bool success) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSetAccountResult);
            p.WriteByte(gender);
            p.WriteBool(success);
            return p.ToArray();
        }
    }
}