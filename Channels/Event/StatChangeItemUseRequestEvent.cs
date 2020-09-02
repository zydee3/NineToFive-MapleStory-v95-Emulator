using System;
using NineToFive.Net;

namespace NineToFive.Event {
    public class StatChangeItemUseRequestEvent : PacketEvent {

        private short _inventoryType;
        private int _itemId;

        public StatChangeItemUseRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time();
            _inventoryType = p.ReadShort();
            _itemId = p.ReadInt();
            return true;
        }

        public override void OnHandle() {
            
        }
    }
}