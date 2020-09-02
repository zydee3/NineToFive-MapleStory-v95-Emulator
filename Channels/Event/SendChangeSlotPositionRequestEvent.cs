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
        private sbyte _oldPos;
        private sbyte _newPos;
        private short _count;
        public SendChangeSlotPositionRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time();
            _inventoryType = p.ReadByte();
            _oldPos = (sbyte) p.ReadShort();
            _newPos = (sbyte) p.ReadShort();
            _count = p.ReadShort();
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            InventoryType inventoryType = _oldPos < 0 ? InventoryType.Equipped : (InventoryType) (_inventoryType - 1);
            Inventory inventory = user.Inventories[inventoryType];

            if (_oldPos < 0) { // un-equipping
                Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.UnequipItem(user.Inventories[InventoryType.Equip], _oldPos, _newPos)));
            } else if (_newPos < 0) { // equipping 
                Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.EquipItem(user.Inventories[InventoryType.Equipped], _oldPos, _newPos)));
            } else if (_newPos == 0) { // dropping item
                Item item = inventory.Remove(_oldPos);
                if (item != null) {
                    item.BagIndex = _oldPos;
                    Drop drop = new Drop(item, user);
                    user.Field.SummonLife(drop);
                    Client.Session.Write(CWvsPackets.GetInventoryOperation(new List<InventoryUpdateEntry>{ new InventoryUpdateEntry(ref item, InventoryOperation.Remove)}));
                }
            } else {
                Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.MoveItem(_oldPos, _newPos)));
            }
        }
    }
}