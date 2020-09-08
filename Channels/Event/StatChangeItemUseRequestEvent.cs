using System;
using System.Collections.Generic;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.Resources;

namespace NineToFive.Event {
    public class StatChangeItemUseRequestEvent : PacketEvent {

        private short _bagIndex;
        private int _itemId;

        public StatChangeItemUseRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time();
            _bagIndex = p.ReadShort();
            _itemId = p.ReadInt();
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            InventoryType type = ItemConstants.GetInventoryType(_itemId);
            Inventory inventory = user.Inventories[type];
            
            List<InventoryUpdateEntry> updates = inventory.UseItem((sbyte) _bagIndex, 1);
            
            if (updates == null || updates.Count == 0) return;
            user.Client.Session.Write(CWvsPackets.GetInventoryOperation(updates));

            InventoryUpdateEntry entry = updates[0];
            //if (entry != null && entry.Complete) entry.Item.ApplyToUser(user);
        }
    }
}