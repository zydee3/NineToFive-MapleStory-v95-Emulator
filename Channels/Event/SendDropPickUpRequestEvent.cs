using System;
using System.Collections.Generic;
using System.Numerics;
using MapleLib.PacketLib;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SendDropPickUpRequestEvent : PacketEvent {

        private uint _objectId;
        
        public SendDropPickUpRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadByte();  // type  
            p.ReadInt();   // update time
            p.ReadShort(); // player location x
            p.ReadShort(); // player location y
            _objectId = p.ReadUInt();   // v9
            p.ReadInt();   // dwID
            
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            Drop drop = user.Field.LifePools[EntityType.Drop][_objectId] as Drop;
            if (drop == null) return;

            InventoryType inventoryType = ItemConstants.GetInventoryType((int) drop.TemplateId);
            Inventory inventory = user.Inventories[inventoryType];
            
            if (drop.Id == 0) {
                
            } else {
                Item item = drop.Item;
                int holdableQuantity = inventory.GetHoldableQuantity(item);
                if (holdableQuantity == 0) { // user inventory is full
                    // send inventory full packet
                    return;
                }
                
                if (holdableQuantity >= item.Quantity) { // user can hold all of it, pick it up and remove from field
                    user.Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.AddItem(item)));
                    user.Field.BroadcastPacket(DropPool.GetDropLeaveField(drop, 2, (int) user.Id));
                    user.Field.RemoveLife(drop);
                } else { // user can hold some, pick up as much as possible and leave rest on the floor
                    user.Client.Session.Write(CWvsPackets.GetInventoryOperation(inventory.AddItem(new Item(item.Id) {Quantity = (ushort) holdableQuantity})));
                    item.Quantity -= (ushort) holdableQuantity;
                }
            }
        }
    }
}