using System;
using System.Collections.Generic;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;
using NineToFive.Util;
using Item = NineToFive.Game.Storage.Item;

namespace NineToFive.Game.Entity {
    public class User : Life, IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(User));
        public readonly AvatarLook AvatarLook;
        public readonly GW_CharacterStat CharacterStat;
        public readonly Dictionary<InventoryType, Inventory> Inventories;
        private Field _field;

        public User(MySqlDataReader reader = null) : base(EntityType.Player) {
            Inventories = new Dictionary<InventoryType, Inventory>();
            foreach (InventoryType type in Enum.GetValues(typeof(InventoryType))) {
                Inventories.Add(type, new Inventory(type));
            }

            AvatarLook = new AvatarLook(reader);
            CharacterStat = new GW_CharacterStat(reader);
            if (reader == null) return;
            AccountId = reader.GetUInt32("account_id");
            using DatabaseQuery q = Database.Table("items");
            using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
            while (r.Read()) {
                int itemId = r.GetInt32("item_id");
                short bagIndex = r.GetInt16("bag_index");
                InventoryType type = ItemConstants.GetInventoryType(itemId);
                if (type == InventoryType.Equip && bagIndex < 0) {
                    Inventories[InventoryType.Equipped].EquipItem(new Equip(itemId, true));
                } else {
                    Item item = new Item(r.GetInt32("item_id")) {
                        GeneratedId = r.GetUInt32("generated_id"),
                        BagIndex = bagIndex,
                        Quantity = r.GetUInt16("quantity"),
                        CashItemSn = r.GetInt64("cash_sn"),
                        DateExpire = r.GetInt64("date_expire")
                    };
                    Inventories[item.InventoryType][item.BagIndex] = item;
                }
            }
        }

        public void Dispose() {
            Save();
            Log.Info($"Saved user '{CharacterStat.Username}'");
            if (Field != null) {
                Field.RemoveLife(this);
                Field = null;
            }
        }

        public uint AccountId { get; set; }
        public Client Client { get; set; }

        public Field Field {
            get => _field;
            set {
                _field = value;
                if (_field != null) CharacterStat.FieldId = _field.Id;
            }
        }

        public void Save() {
            using DatabaseQuery updateChars = Database.Table("characters");
            int count = updateChars.Update(Database.CreateUserParameters(this))
                .Where("id", "=", CharacterStat.Id)
                .ExecuteNonQuery();
            if (count == 0) throw new InvalidOperationException($"Failed to save character({CharacterStat.Username})");

            using DatabaseQuery deleteItems = Database.Table("items");
            count = deleteItems.Delete().ExecuteNonQuery();
            Log.Info($"Deleted {count} items from {CharacterStat.Username}'s inventories");

            using DatabaseQuery insertItems = Database.Table("items");
            foreach (var inventory in Inventories.Values) {
                foreach (var item in inventory.Items) {
                    insertItems.Insert(Database.CreateItemParameters(this, item));
                }
            }

            count = insertItems.ExecuteNonQuery();
            Log.Info($"Successfully saved {count} items in {CharacterStat.Username}'s inventories");
        }

        public void SetField(in int fieldId, bool characterData = true) {
            Field?.RemoveLife(this);
            Field = Client.Channel.GetField(fieldId);
            Field.AddLife(this);

            using Packet w = new Packet();
            w.WriteShort((short) CStage.OnSetField);

            // CClientOptMan::DecodeOpt
            for (int i = 0; i < w.WriteShort(); i++) {
                w.WriteInt();
                w.WriteInt();
            }

            w.WriteInt(Math.Abs(Field.VRRight) - Math.Abs(Field.VRLeft)); // nFieldWidth
            w.WriteInt(Math.Abs(Field.VRBottom) - Math.Abs(Field.VRTop)); // nFieldHeight
            w.WriteByte(1);                                               // unknown
            w.WriteBool(characterData);
            short notifierCheck = w.WriteShort();
            if (notifierCheck > 0) {
                w.WriteString();
                for (int i = 0; i < notifierCheck; i++) {
                    w.WriteString();
                }
            }

            if (characterData) {
                // CalcDamage::SetSeed
                w.WriteInt(RNG.GetInt());
                w.WriteInt(RNG.GetInt());
                w.WriteInt(RNG.GetInt());

                UserPackets.EncodeCharacterData(this, w, -1);
                // CWvsContext::OnSetLogoutGiftConfig
                w.WriteInt();
                for (int i = 0; i < 3; i++) {
                    w.WriteInt();
                }
            } else {
                w.WriteBool(false); // CWvsContext::OnRevive
                w.WriteInt(CharacterStat.FieldId);
                w.WriteByte(CharacterStat.Portal);
                w.WriteInt(CharacterStat.HP);
                if (w.WriteBool(false)) {
                    w.WriteInt();
                    w.WriteInt();
                }
            }

            w.WriteLong(DateTime.Now.ToFileTime()); // paramFieldInit.ftServer
            Client.Session.Write(w.ToArray());
        }
    }

    public class AvatarLook : IPacketSerializer<User> {
        public byte Gender { get; set; }
        public byte Skin { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }

        public AvatarLook(MySqlDataReader r = null) {
            if (r == null) return;
            Gender = r.GetByte("gender");
            Skin = r.GetByte("skin");
            Face = r.GetInt32("face");
            Hair = r.GetInt32("hair");
        }

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
        public sbyte Level { get; set; } = 1;
        public short Job { get; set; }
        public short Str { get; set; } = 4;
        public short Dex { get; set; } = 4;
        public short Int { get; set; } = 4;
        public short Luk { get; set; } = 4;
        public int HP { get; set; } = 50;
        public int MaxHP { get; set; } = 50;
        public int MP { get; set; } = 5;
        public int MaxMP { get; set; } = 5;
        public short AP { get; set; }
        public short[] SP { get; set; } = new short[10];
        public int Exp { get; set; }
        public short Popularity { get; set; }
        public int FieldId { get; set; } = 10000;
        public byte Portal { get; set; }

        public GW_CharacterStat(MySqlDataReader r = null) {
            if (r == null) return;
            Id = r.GetUInt32("character_id");
            Username = r.GetString("username");
            Level = r.GetSByte("level");
            Job = r.GetInt16("job");
            Str = r.GetInt16("str");
            Dex = r.GetInt16("dex");
            Int = r.GetInt16("int");
            Luk = r.GetInt16("luk");
            HP = r.GetInt32("hp");
            MaxHP = r.GetInt32("max_hp");
            MP = r.GetInt32("mp");
            MaxMP = r.GetInt32("max_mp");
            AP = r.GetInt16("ability_points");
            Exp = r.GetInt32("exp");
            Popularity = r.GetInt16("popularity");
            FieldId = r.GetInt32("field_id");
            Portal = r.GetByte("portal");
        }

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
            if (NineToFive.Job.IsExtendedSpJob(Job)) {
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