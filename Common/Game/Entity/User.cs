using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Constants;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;
using NineToFive.Util;

namespace NineToFive.Game.Entity {
    public class User : Life {
        private static readonly ILog Log = LogManager.GetLogger(typeof(User));

        public User(MySqlDataReader reader = null) : base(EntityType.User) {
            Inventories = new Dictionary<InventoryType, Inventory>();
            foreach (InventoryType type in Enum.GetValues(typeof(InventoryType))) {
                Inventories.Add(type, new Inventory(type));
            }

            AvatarLook = new AvatarLook(reader);
            CharacterStat = new CharacterStat(reader);
            if (reader == null) return;
            AccountId = reader.GetUInt32("account_id");
            using DatabaseQuery q = Database.Table("items");
            using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
            while (r.Read()) {
                int itemId = r.GetInt32("item_id");
                short bagIndex = r.GetInt16("bag_index");
                InventoryType type = ItemConstants.GetInventoryType(itemId);
                Item item;

                if (type == InventoryType.Equip) {
                    if (bagIndex < 0) {
                        Inventories[InventoryType.Equipped].EquipItem(new Equip(itemId, true));
                        continue;
                    }

                    item = new Equip(itemId);
                } else {
                    item = new Item(itemId);
                    item.Quantity = r.GetUInt16("quantity");
                }

                item.GeneratedId = r.GetUInt32("generated_id");
                item.BagIndex = bagIndex;
                item.CashItemSn = r.GetInt64("cash_sn");
                item.DateExpire = r.GetInt64("date_expire");
                Inventories[item.InventoryType][item.BagIndex] = item;
            }
        }

        public uint AccountId { get; set; }
        public Client Client { get; set; }
        public byte GradeCode { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDebugging { get; set; }
        public AvatarLook AvatarLook { get; }
        public CharacterStat CharacterStat { get; }
        public Dictionary<InventoryType, Inventory> Inventories { get; }

        public override Field Field {
            get => base.Field;
            set {
                base.Field = value;
                if (value != null) CharacterStat.FieldId = value.Id;
            }
        }

        public override void Dispose() {
            Save();
            Log.Info($"Saved user '{CharacterStat.Username}'");

            base.Dispose();
            Client.World.Users.TryRemove(CharacterStat.Id, out _);
        }

        public override byte[] EnterFieldPacket() {
            using Packet w = new Packet();
            w.WriteShort((short) CUserPool.OnUserEnterField);
            w.WriteUInt(CharacterStat.Id);
            UserPackets.EncodeUserRemoteInit(this, w);
            return w.ToArray();
        }

        public override byte[] LeaveFieldPacket() {
            using Packet w = new Packet();
            w.WriteShort((short) CUserPool.OnUserLeaveField);
            w.WriteUInt(CharacterStat.Id);
            return w.ToArray();
        }

        public void Save() {
            using DatabaseQuery updateChars = Database.Table("characters");
            int count = updateChars.Update(Database.CreateUserParameters(this))
                .Where("character_id", "=", CharacterStat.Id)
                .ExecuteNonQuery();
            if (count == 0) throw new InvalidOperationException($"Failed to save character({CharacterStat.Username})");

            using DatabaseQuery deleteItems = Database.Table("items");
            count = deleteItems.Delete().ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Cleaned up {count} items");

            using DatabaseQuery insertItems = Database.Table("items");
            foreach (var inventory in Inventories.Values) {
                foreach (var item in inventory.Items) {
                    insertItems.Insert(Database.CreateItemParameters(this, item));
                }
            }

            count = insertItems.ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Saved {count} items");
        }

        /// <summary>
        /// changes the user's field
        /// <para>a portal is typically specified when the character is transferring to a new field via entering a portal</para>
        /// </summary>
        /// <param name="fieldId">destination field</param>
        /// <param name="portal">source portal</param>
        /// <param name="characterData">to re-encode character data (a refresh essentially)</param>
        public void SetField(int fieldId, Portal portal = null, bool characterData = true) {
            Field?.RemoveLife(this);
            Field = Client.Channel.GetField(fieldId);
            if (portal != null) {
                var destPortal = Field.Portals.First(p => p.Name.Equals(portal.Name));
                Location = destPortal.Location;
                CharacterStat.Portal = portal.Id;
            }

            using Packet w = new Packet();
            w.WriteShort((short) CStage.OnSetField);

            // CClientOptMan::DecodeOpt
            for (int i = 0; i < w.WriteShort(); i++) {
                w.WriteInt();
                w.WriteInt();
            }

            w.WriteInt(Math.Abs(Field.VrRight) - Math.Abs(Field.VrLeft)); // nFieldWidth
            w.WriteInt(Math.Abs(Field.VrBottom) - Math.Abs(Field.VrTop)); // nFieldHeight
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
                w.WriteInt(Randomizer.GetInt());
                w.WriteInt(Randomizer.GetInt());
                w.WriteInt(Randomizer.GetInt());

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

            Field.AddLife(this);
            // let people know this person entered the field
            Field.BroadcastPacketExclude(this, EnterFieldPacket());
            // let this person know other things are in the field
            foreach (EntityType type in Enum.GetValues(typeof(EntityType))) {
                foreach (Life life in Field.LifePools[type].Values) {
                    if (life == this) continue;
                    Client.Session.Write(life.EnterFieldPacket());
                }
            }
        }

        public void SendMessage(string msg, byte type = 5) {
            Client.Session.Write(CWvsPackets.GetBroadcastMessage(this, true, type, $"[NineToFive] {msg}", null));
        }
    }

    public class AvatarLook : IPacketSerializer<User> {
        public AvatarLook(MySqlDataReader r = null) {
            if (r == null) return;
            Gender = r.GetByte("gender");
            Skin = r.GetByte("skin");
            Face = r.GetInt32("face");
            Hair = r.GetInt32("hair");
        }

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

    public class CharacterStat : IPacketSerializer<User> {
        public CharacterStat(MySqlDataReader r = null) {
            if (r == null) return;
            Id = r.GetUInt32("character_id");
            Username = r.GetString("username");
            Level = r.GetByte("level");
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

        public uint Id { get; set; }
        public string Username { get; set; }
        public byte Level { get; set; } = 1;
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

        public void SendUpdate(User user, uint dwcharFlags) {
            user.Client.Session.Write(CWvsPackets.GetStatChanged(user, dwcharFlags));
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

            p.WriteByte(Level);
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
            if (JobConstants.IsExtendedSpJob(Job)) {
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

        public void EncodeChangeStat(User user, Packet p, uint dwcharFlag) {
            p.WriteUInt(dwcharFlag);
            if ((dwcharFlag & 1) == 1) p.WriteByte(user.AvatarLook.Skin);
            if ((dwcharFlag & 4) == 4) p.WriteInt(user.AvatarLook.Face);
            if ((dwcharFlag & 2) == 2) p.WriteInt(user.AvatarLook.Hair);
            if ((dwcharFlag & 8) == 8) p.WriteLong(); // pet 1
            if ((dwcharFlag & 0x80000) == 0x80000) p.WriteLong(); // pet 2
            if ((dwcharFlag & 0x100000) == 0x100000) p.WriteLong(); // pet 3
            if ((dwcharFlag & 0x10) == 0x10) p.WriteByte(Level);
            if ((dwcharFlag & 0x20) == 0x20) p.WriteShort(Job);
            if ((dwcharFlag & 0x40) == 0x40) p.WriteShort(Str);
            if ((dwcharFlag & 0x80) == 0x80) p.WriteShort(Dex);
            if ((dwcharFlag & 0x100) == 0x100) p.WriteShort(Int);
            if ((dwcharFlag & 0x200) == 0x200) p.WriteShort(Luk);
            if ((dwcharFlag & 0x400) == 0x400) p.WriteInt(HP);
            if ((dwcharFlag & 0x800) == 0x800) p.WriteInt(MaxHP);
            if ((dwcharFlag & 0x1000) == 0x1000) p.WriteInt(MP);
            if ((dwcharFlag & 0x2000) == 0x2000) p.WriteInt(MaxMP);
            if ((dwcharFlag & 0x4000) == 0x4000) p.WriteShort(AP);

            if ((dwcharFlag & 0x8000) == 0x8000) {
                if (JobConstants.IsExtendedSpJob(Job)) {
                    byte advancements = (byte) (9 - Math.Min(9, 2218 - Job));
                    p.WriteByte(advancements);
                    for (byte i = 0; i < advancements; i++) {
                        p.WriteByte(i);
                        p.WriteByte((byte) SP[i]);
                    }
                } else {
                    p.WriteShort(SP[0]);
                }
            }

            if ((dwcharFlag & 0x10000) == 0x10000) p.WriteInt(Exp);
            if ((dwcharFlag & 0x20000) == 0x20000) p.WriteShort(Popularity);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt();
            if ((dwcharFlag & 0x200000) == 0x200000) p.WriteInt(FieldId);
        }

        public void Decode(User user, Packet p) { }
    }
}