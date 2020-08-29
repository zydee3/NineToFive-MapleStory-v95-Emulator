﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Microsoft.ClearScript.V8;
using MySql.Data.MySqlClient;
using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
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

            Skills = new Dictionary<int, SkillRecord>();
            KeyMap = new Dictionary<int, Tuple<byte, int>>(89);

            AccountId = reader.GetUInt32("account_id");
            Money = reader.GetUInt32("money");

            using (DatabaseQuery q = Database.Table("items")) {
                using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
                while (r.Read()) {
                    int itemId = r.GetInt32("item_id");
                    short bagIndex = r.GetInt16("bag_index");
                    InventoryType type = ItemConstants.GetInventoryType(itemId);
                    Item item;

                    if (type == InventoryType.Equip) {
                        if (bagIndex < 0) {
                            Inventories[InventoryType.Equipped].EquipItem(new Equip(itemId, true, true));
                            continue;
                        }

                        item = new Equip(itemId, false, true);
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

            using (DatabaseQuery q = Database.Table("skill_records")) {
                using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
                while (r.Read()) {
                    var record = new SkillRecord(r.GetInt32("skill_id"), r.GetInt32("skill_level")) {
                        Expiration = r.GetInt64("date_expire"),
                        MasterLevel = r.GetInt32("master_level"),
                    };
                    Skills.Add(record.Id, record);
                }
            }

            using (DatabaseQuery q = Database.Table("keymap")) {
                using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
                while (r.Read()) {
                    KeyMap[r.GetInt32("key")] = new Tuple<byte, int>(r.GetByte("type"), r.GetInt32("value"));
                }
            }
        }

        public uint AccountId { get; set; }
        public Client Client { get; set; }
        public byte GradeCode { get; set; }
        public bool IsHidden { get; set; }
        public uint Money { get; set; }
        public bool IsDebugging { get; set; }
        public AvatarLook AvatarLook { get; }
        public CharacterStat CharacterStat { get; }
        public Dictionary<InventoryType, Inventory> Inventories { get; }
        public Dictionary<int, Tuple<byte, int>> KeyMap { get; set; }
        public int[] QuickslotKeyMap { get; set; }
        public Dictionary<int, SkillRecord> Skills { get; }
        public V8ScriptEngine ScriptEngine { get; set; }

        public override Field Field {
            get => base.Field;
            set {
                base.Field = value;
                if (value != null) CharacterStat.FieldId = value.Id;
            }
        }

        public override void Dispose() {
            Save();
            Log.Info($"[Dispose] {CharacterStat.Username} : Completed save");

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

            #region items

            using DatabaseQuery deleteItems = Database.Table("items");
            count = deleteItems.Where("character_id", "=", CharacterStat.Id).Delete().ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Cleaned up {count} items");

            using DatabaseQuery insertItems = Database.Table("items");
            foreach (var inventory in Inventories.Values) {
                foreach (var item in inventory.Items) {
                    insertItems.Insert(Database.CreateItemParameters(this, item));
                }
            }

            count = insertItems.ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Saved {count} items");

            #endregion

            #region skills

            using DatabaseQuery deleteSkills = Database.Table("skill_records");
            count = deleteSkills.Where("character_id", "=", CharacterStat.Id).Delete().ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Cleaned up {count} skill records");

            if (Skills.Count > 0) {
                using DatabaseQuery insertSkills = Database.Table("skill_records");
                foreach (var pair in Skills) {
                    insertSkills.Insert(Database.CreateSkillParameters(this, pair));
                }

                count = insertSkills.ExecuteNonQuery();
                Log.Info($"[Save] {CharacterStat.Username} : Saved {count} skill records");
            }

            #endregion

            #region keymap

            using DatabaseQuery deleteKeymap = Database.Table("keymap");
            count = deleteKeymap.Where("character_id", "=", CharacterStat.Id).Delete().ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Cleaned up {count} key mappings");

            if (KeyMap.Count > 0) {
                using DatabaseQuery insertKeymap = Database.Table("keymap");
                foreach (var pair in KeyMap) {
                    insertKeymap.Insert("character_id", CharacterStat.Id, "key", pair.Key, "type", pair.Value.Item1, "value", pair.Value.Item2);
                }

                count = insertKeymap.ExecuteNonQuery();
                Log.Info($"[Save] {CharacterStat.Username} : Saved {count} key mappings");
            }

            #endregion
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
                Location = portal.Location;
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

                    // update controller if one doesn't exist; TryGetTarget returns false when 
                    // the Controller target is null
                    if (life is Mob mob && !mob.Controller.TryGetTarget(out _)) {
                        mob.UpdateController(this);
                    }
                }
            }
        }

        public void SendMessage(string msg, byte type = 5) {
            Client.Session.Write(CWvsPackets.GetBroadcastMessage(this, true, type, $"[NineToFive] {msg}", null));
        }

        public bool GainMoney(int gain) {
            int balance = (int) Money + gain;
            if (balance >= 0) {
                Money = (uint) balance;
                CharacterStat.SendUpdate(this, (uint) UserAbility.Money);
                return true;
            }

            return false;
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
            p.WriteByte(user.AvatarLook.Gender);
            p.WriteByte(user.AvatarLook.Skin);
            p.WriteInt(user.AvatarLook.Face);
            p.WriteByte();
            p.WriteInt(user.AvatarLook.Hair);
            var inventory = user.Inventories[InventoryType.Equipped];
            foreach (Item item in inventory.Items.Where(i => i.BagIndex >= -99)) {
                p.WriteByte((byte) Math.Abs(item.BagIndex));
                p.WriteInt(item.Id);
            }

            p.WriteByte(255);
            foreach (Item item in inventory.Items.Where(i => i.BagIndex <= -100)) {
                p.WriteByte((byte) item.BagIndex);
                p.WriteInt(item.Id);
            }

            p.WriteByte(255);
            p.WriteInt(inventory[-11]?.Id ?? 0);
            p.WriteBytes(new byte[12]); // pets
        }

        public void Decode(User user, Packet p) {
            user.AvatarLook.Gender = p.ReadByte();
            user.AvatarLook.Skin = p.ReadByte();
            user.AvatarLook.Face = p.ReadInt();
            user.AvatarLook.Hair = p.ReadInt();
        }
    }

    public class CharacterStat : IPacketSerializer<User> {
        private readonly short[] _skillPoints;
        private int _hp = 50, _mp = 5;
        private uint _exp;

        public CharacterStat(MySqlDataReader r = null) {
            _skillPoints = new short[10];
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
            Exp = r.GetUInt32("exp");
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

        public int MaxHP { get; set; } = 50;

        public int MP {
            get => _mp;
            set => _mp = Math.Min(Math.Max(value, 0), MaxMP);
        }

        public int HP {
            get => _hp;
            set => _hp = Math.Min(Math.Max(value, 0), MaxHP);
        }

        public uint Exp {
            get => _exp;
            set {
                uint needed = GameConstants.GetExpToLevel(Level);
                _exp = Math.Min(Math.Max(needed, 0), value);
                while (_exp >= needed) {
                    _exp -= needed;
                    Level++;
                }
            }
        }

        public int MaxMP { get; set; } = 5;
        public short AP { get; set; }

        public short SP {
            get {
                byte index = 0;
                if (JobConstants.IsExtendedSpJob(Job)) {
                    index = (byte) (9 - Math.Min(9, 2218 - Job));
                }

                return _skillPoints[index];
            }
            set {
                byte index = 0;
                if (JobConstants.IsExtendedSpJob(Job)) {
                    index = (byte) (9 - Math.Min(9, 2218 - Job));
                }

                _skillPoints[index] = value;
            }
        }

        public short[] SkillPoints => _skillPoints;
        public short Popularity { get; set; }
        public int FieldId { get; set; } = 10000;
        public byte Portal { get; set; }

        public void SendUpdate(User user, uint dwcharFlags) {
            if (((UserAbility) dwcharFlags & UserAbility.HP) == UserAbility.HP) {
                if (user.CharacterStat.HP < 1) {
                    user.SendMessage("You have died.");
                }
            }

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

            p.WriteByte(user.CharacterStat.Level);
            var jobId = p.WriteShort(user.CharacterStat.Job);
            p.WriteShort(user.CharacterStat.Str);
            p.WriteShort(user.CharacterStat.Dex);
            p.WriteShort(user.CharacterStat.Int);
            p.WriteShort(user.CharacterStat.Luk);
            p.WriteInt(user.CharacterStat.HP);
            p.WriteInt(user.CharacterStat.MaxHP);
            p.WriteInt(user.CharacterStat.MP);
            p.WriteInt(user.CharacterStat.MaxMP);
            p.WriteShort(user.CharacterStat.AP);

            if (JobConstants.IsExtendedSpJob(jobId)) {
                byte advancements = (byte) (9 - Math.Min(9, 2218 - jobId));
                p.WriteByte(advancements);
                for (byte i = 0; i < advancements; i++) {
                    p.WriteByte(i);
                    p.WriteByte((byte) SkillPoints[i]);
                }
            } else {
                p.WriteShort(SP);
            }

            p.WriteUInt(Exp);
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
            if ((dwcharFlag & 8) == 8) p.WriteLong();               // pet 1
            if ((dwcharFlag & 0x80000) == 0x80000) p.WriteLong();   // pet 2
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
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt((int) user.Money);

            if ((dwcharFlag & 0x8000) == 0x8000) {
                if (JobConstants.IsExtendedSpJob(Job)) {
                    byte advancements = (byte) (9 - Math.Min(9, 2218 - Job));
                    p.WriteByte(advancements);
                    for (byte i = 0; i < advancements; i++) {
                        p.WriteByte(i);
                        p.WriteByte((byte) SkillPoints[i]);
                    }
                } else {
                    p.WriteShort(SP);
                }
            }

            if ((dwcharFlag & 0x10000) == 0x10000) p.WriteUInt(Exp);
            if ((dwcharFlag & 0x20000) == 0x20000) p.WriteShort(Popularity);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt();
            if ((dwcharFlag & 0x200000) == 0x200000) p.WriteInt(FieldId);
        }

        public void Decode(User user, Packet p) {
            user.CharacterStat.Id = p.ReadUInt();
            user.CharacterStat.Username = p.ReadString(13).Trim();
            user.AvatarLook.Gender = p.ReadByte();
            user.AvatarLook.Skin = p.ReadByte();
            user.AvatarLook.Face = p.ReadInt();
            user.AvatarLook.Hair = p.ReadInt();
            p.ReadLong();
            p.ReadLong();
            p.ReadLong();
            user.CharacterStat.Level = p.ReadByte();
            var jobId = (user.CharacterStat.Job = p.ReadShort());
            user.CharacterStat.Str = p.ReadShort();
            user.CharacterStat.Dex = p.ReadShort();
            user.CharacterStat.Int = p.ReadShort();
            user.CharacterStat.Luk = p.ReadShort();
            user.CharacterStat._hp = p.ReadInt();
            user.CharacterStat.MaxHP = p.ReadInt();
            user.CharacterStat._mp = p.ReadInt();
            user.CharacterStat.MaxMP = p.ReadInt();
            user.CharacterStat.AP = p.ReadShort();

            if (JobConstants.IsExtendedSpJob(jobId)) {
                byte advancements = p.ReadByte();
                for (int i = 0; i < advancements; i++) {
                    user.CharacterStat.SkillPoints[p.ReadByte()] = p.ReadByte();
                }
            } else {
                user.CharacterStat.SP = p.ReadShort();
            }

            user.CharacterStat.Exp = p.ReadUInt();
            user.CharacterStat.Popularity = p.ReadShort();
            p.ReadInt();
            user.CharacterStat.FieldId = p.ReadInt();
            user.CharacterStat.Portal = p.ReadByte();
            p.ReadInt();
            p.ReadShort();
        }
    }
}