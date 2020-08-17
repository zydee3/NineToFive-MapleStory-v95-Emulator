using System;
using System.Collections.Generic;
using NineToFive.Net;

namespace NineToFive.Event {
    public class SaveFuncKeyMapEvent : PacketEvent {
        private int _action;
        private int _itemId;
        private Dictionary<int, Tuple<byte, int>> _keyMaps;

        public SaveFuncKeyMapEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            switch (_action = p.ReadInt()) {
                case 0: // CFuncKeyMappedMan::SaveFuncKeyMap
                    int count = p.ReadInt();
                    _keyMaps = new Dictionary<int, Tuple<byte, int>>(count);
                    for (int i = 0; i < count; i++) {
                        _keyMaps.Add(p.ReadInt(), new Tuple<byte, int>(p.ReadByte(), p.ReadInt()));
                    }

                    return true;
                case 1: // CFuncKeyMappedMan::ChangePetConsumeItemID
                    _itemId = p.ReadInt();
                    return true;
                case 2: // CFuncKeyMappedMan::ChangePetConsumeMPItemID
                    _itemId = p.ReadInt();
                    return true;
            }

            return false;
        }

        public override void OnHandle() {
            switch (_action) {
                case 0:
                    Client.User.KeyMap = _keyMaps;
                    break;
            }
        }
    }
}