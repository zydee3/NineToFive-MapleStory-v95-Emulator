using NineToFive.Event;
using NineToFive.IO;
using NineToFive.SendOps;

namespace NineToFive.Login.Event {
    class PinCodeResultEvent : PacketEvent {
        /// <summary>
        /// typically, represents the value of CPinCodeDlg::EnterPinCode but mostly hard-coded as 1 due to Nexon
        /// no longer using this system
        /// </summary>
        private byte _result;

        private byte _unk0;
        private string _unk1;

        public PinCodeResultEvent(Client client) : base(client) { }

        public override void OnHandle() {
            Client.Session.Write(GetCheckPinCodeResult(_result));
        }

        public override bool OnProcess(Packet p) {
            _result = p.ReadByte();
            if (_result == 1) {
                // create pin-code
                _result = 0;
            }

            _unk0 = p.ReadByte();
            _unk1 = p.ReadString();
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
            p.WriteShort((short) CLogin.OnCheckPinCodeResult);
            p.WriteByte();
            return p.ToArray();
        }
    }
}