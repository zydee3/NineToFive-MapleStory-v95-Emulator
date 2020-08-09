using log4net;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SaveQuickSlotKeyMapEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SaveQuickSlotKeyMapEvent));
        private int[] _keymap;

        public SaveQuickSlotKeyMapEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _keymap = new int[8];
            for (int i = 0; i < 8; i++) {
                _keymap[i] = p.ReadInt();
            }

            return true;
        }

        public override void OnHandle() {
            Client.User.QuickslotKeyMap = _keymap;
        }

        public static byte[] GetQuickSlotInit(byte[] quickSlot = null) {
            using Packet w = new Packet();
            w.WriteShort((short) CField_QuickslotKeyMappedMan.OnInit);
            bool def = quickSlot == null;
            if (!w.WriteBool(def)) {
                foreach (var id in quickSlot!) {
                    w.WriteInt(id);
                }
            }

            return w.ToArray();
        }
    }
}