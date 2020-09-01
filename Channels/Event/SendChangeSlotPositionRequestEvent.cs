using System;
using System.Collections.Generic;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
using NineToFive.Net;
using NineToFive.Packets;

namespace NineToFive.Event {
    public class SendChangeSlotPositionRequestEvent : PacketEvent {

        private byte _inventoryType;
        private short _oldPos;
        private short _newPos;
        private short _count;
        public SendChangeSlotPositionRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time();
            _inventoryType = p.ReadByte();
            _oldPos = p.ReadShort();
            _newPos = p.ReadShort();
            _count = p.ReadShort();
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            InventoryType inventoryType = (InventoryType) (_inventoryType - 1);
            Inventory inventory = user.Inventories[inventoryType];

            if (_oldPos < 0) {
                Item equip = inventory.Remove((byte) _oldPos);
            } else if (_newPos < 0) {
                Item equip = inventory.Remove((byte) _oldPos);
            } else if (_newPos == 0) {
                Item item = inventory.Remove((byte) _oldPos);
                if (item != null) {
                    item.BagIndex = _oldPos;
                    Drop drop = new Drop(item, user);
                    user.Field.SummonLife(drop);
                    Client.Session.Write(CWvsPackets.GetInventoryOperation(new List<InventoryUpdateEntry>{ new InventoryUpdateEntry(ref item, InventoryOperation.Remove)}));
                }
            } else {
                Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.MoveItem((byte)_oldPos, (byte)_newPos)));
            }
        }
    }
}