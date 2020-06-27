using NineToFive.IO;

namespace NineToFive.Event {
    class PinCodeResultEvent : PacketEvent {
        /// <summary>
        /// typically, represents the value of CPinCodeDlg::EnterPinCode but mostly hard-coded as 1 due to Nexon
        /// no longer using this system
        /// </summary>
        byte result;

        byte unk0;
        string unk1;

        public PinCodeResultEvent(Client client) : base(client) { }

        public override void OnHandle() {
            Client.Session.Write(GetCheckPinCodeResult(result));
        }

        public override bool OnProcess(Packet p) {
            result = p.ReadByte();
            if (result == 1) { // create pin-code
                result = 0;
            }
            unk0 = p.ReadByte();
            unk1 = p.ReadString();
            return true;
        }

        /// <summary>
        /// <para>0    for world list packet</para>
        /// <para>1    for CPinCodeDlg::CreatePinCode</para>
        /// <para>2,4  for CPinCodeDlg::EnterPinCode</para>
        /// <para>3    for CLoginUtilDlg::Error</para>
        /// <para>7    for CLogin::GotoTitle</para>
        /// </summary>
        /// <param name="a">result of the pin code</param>
        private static byte[] GetCheckPinCodeResult(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) SendOps.CLogin.OnCheckPinCodeResult);
            p.WriteByte();
            return p.ToArray();
        }
    }
}