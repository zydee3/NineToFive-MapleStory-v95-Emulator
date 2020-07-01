using System;
using System.Collections.Generic;
using NineToFive.Game.Storage;
using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Game {
    public class User {
        public readonly AvatarLook AvatarLook = new AvatarLook();
        public readonly GW_CharacterStat CharacterStat = new GW_CharacterStat();
        public readonly Dictionary<InventoryType, Inventory> Inventories;

        public User() {
            Inventories = new Dictionary<InventoryType, Inventory>();
            foreach (InventoryType type in Enum.GetValues(typeof(InventoryType))) {
                Inventories.Add(type, new Inventory(type));
            }
        }

        public Client Client { get; set; }
    }

    public class AvatarLook : IPacketSerializer<User> {
        public byte Gender { get; set; }
        public byte Skin { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }

        public void Encode(User user, Packet p) {
            p.WriteByte(Gender);
            p.WriteByte(Skin);
            p.WriteInt(Face);
            p.WriteByte();
            p.WriteInt(Hair);
            var inventory = user.Inventories[InventoryType.Equipped];
            foreach (Item item in inventory.Items) {
                p.WriteByte((byte) Math.Abs(item.BagIndex));
                p.WriteInt(item.Id);
            }

            p.WriteByte(255);
            foreach (Item item in inventory.Items) {
                p.WriteByte((byte) item.BagIndex);
                p.WriteInt(item.Id);
            }

            p.WriteByte(255);
            p.WriteInt(inventory[-11]?.Id ?? 0);
            p.WriteBytes(new byte[12]); // pets
        }

        public void Decode(User user, Packet p) { }
    }

    public class GW_CharacterStat : IPacketSerializer<User> {
        public uint Id { get; set; }
        public string Username { get; set; }
        public sbyte Level { get; set; }
        public short Job { get; set; }
        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public short AP { get; set; }
        public short[] SP { get; set; } = new short[10];
        public int Exp { get; set; }
        public short Popularity { get; set; }
        public int FieldId { get; set; }
        public byte Portal { get; set; }


        public void Encode(User user, Packet p) {
            if (Id == 0) throw new InvalidOperationException("cannot encode a character which id is 0");
            p.WriteUInt(Id);
            p.WriteStringFixed(Username, 13);
            p.WriteByte(user.AvatarLook.Gender);
            p.WriteByte(user.AvatarLook.Skin);
            p.WriteInt(user.AvatarLook.Face);
            p.WriteInt(user.AvatarLook.Hair);
            // pets
            p.WriteLong();
            p.WriteLong();
            p.WriteLong();

            p.WriteSByte(Level);
            p.WriteShort(Job);
            p.WriteShort(Str);
            p.WriteShort(Dex);
            p.WriteShort(Int);
            p.WriteShort(Luk);
            p.WriteInt(HP);
            p.WriteInt(MaxHP);
            p.WriteInt(MP);
            p.WriteInt(MaxMP);
            p.WriteShort(AP);
            if (Constants.Job.IsExtendedSpJob(Job)) {
                byte advancements = (byte) (9 - Math.Min(9, 2218 - Job));
                p.WriteByte(advancements);
                for (byte i = 0; i < advancements; i++) {
                    p.WriteByte(i);
                    p.WriteByte((byte) SP[i]);
                }
            } else {
                p.WriteShort(SP[0]);
            }

            p.WriteInt(Exp);
            p.WriteShort(Popularity);
            p.WriteInt();
            p.WriteInt(FieldId);
            p.WriteByte(Portal);
            p.WriteInt();
            p.WriteShort();
        }

        public void Decode(User user, Packet p) { }
    }
}