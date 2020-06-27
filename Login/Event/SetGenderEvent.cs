using NineToFive.IO;

namespace NineToFive.Event {
    /// <summary>
    /// <para>CLogin::SendCancelGenderPacket</para>
    /// <para>CLogin::SendSetGenderPacket</para>
    /// </summary>
    class SetGenderEvent : PacketEvent {

        private bool success;
        private byte gender;

        public SetGenderEvent(Client client) : base(client) {
        }

        public override void OnHandle() {
            if (success) Client.Gender = gender;
            Client.Session.Write(GetSetAccountResult(gender, success));
        }

        public override bool OnProcess(Packet p) {
            success = p.ReadBool();
            if (success) gender = p.ReadByte();
            return gender == 0 || gender == 1;
        }

        private static byte[] GetSetAccountResult(byte gender, bool success) {
            using Packet p = new Packet();
            p.WriteShort((short)SendOps.CLogin.OnSetAccountResult);
            p.WriteByte(gender);
            p.WriteBool(success);
            return p.ToArray();
        }
    }
}