using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

            var me = this;
            AvatarLook = new AvatarLook(ref me, reader);
            CharacterStat = new CharacterStat(ref me, reader);

            if (reader == null) return;

            ForcedStat = new ForcedStat();
            Skills = new Dictionary<int, SkillRecord>();
            KeyMap = new Dictionary<int, Tuple<byte, int>>(89);

            AccountId = reader.GetUInt32("account_id");
            Money = reader.GetUInt32("money");

            var equips = new Dictionary<int, ItemSlotEquip>();
            using (DatabaseQuery q = Database.Table("items")) {
                using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
                while (r.Read()) {
                    int itemId = r.GetInt32("item_id");
                    short bagIndex = r.GetInt16("bag_index");
                    InventoryType type = ItemConstants.GetInventoryType(itemId);
                    ItemSlot itemSlot;
                    
                    if (type == InventoryType.Equip) {
                        itemSlot = new ItemSlotEquip(itemId);
                        equips.TryAdd(bagIndex, (ItemSlotEquip) itemSlot);
                    } else {
                        itemSlot = new ItemSlotBundle(itemId, r.GetUInt16("quantity"));
                    }

                    itemSlot.GeneratedId = r.GetUInt32("generated_id"); 
                    itemSlot.CashItemSN = r.GetInt64("cash_sn");
                    itemSlot.DateExpire = r.GetInt64("date_expire");
                    itemSlot.BagIndex = bagIndex;
                    
                    if (bagIndex < 0) {
                        Inventories[InventoryType.Equipped].EquipItem((ItemSlotEquip) itemSlot);
                    } else {
                        Inventories[itemSlot.InventoryType][itemSlot.BagIndex] = itemSlot;    
                    }
                }
            }

            using (DatabaseQuery q = Database.Table("equips")) {
                using MySqlDataReader r = q.Select().Where("character_id", "=", CharacterStat.Id).ExecuteReader();
                while (r.Read()) {
                    if (!equips.TryGetValue(r.GetInt32("bag_index"), out ItemSlotEquip equip)) continue;
                    equip.MaxHP = r.GetInt16("hp");
                    equip.MaxHPR = r.GetInt16("hpr");
                    equip.MaxMP = r.GetInt16("mp");
                    equip.MaxMPR = r.GetInt16("mpr");
                    equip.STR = r.GetInt16("str");
                    equip.DEX = r.GetInt16("dex");
                    equip.INT = r.GetInt16("int");
                    equip.LUK = r.GetInt16("luk");
                    equip.PAD = r.GetInt16("pad");
                    equip.MAD = r.GetInt16("mad");
                    equip.PDD = r.GetInt16("pdd");
                    equip.MDD = r.GetInt16("mdd");
                    equip.ACC = r.GetInt16("acc");
                    equip.EVA = r.GetInt16("eva");
                    equip.Craft = r.GetInt16("craft");
                    equip.Speed = r.GetInt16("speed");
                    equip.Jump = r.GetInt16("jump");
                    equip.Title = r.GetString("title");
                    equip.Durability = r.GetInt32("durability");
                    equip.Grade = r.GetByte("grade");
                    equip.Option1 = r.GetInt16("option_1");
                    equip.Option2 = r.GetInt16("option_2");
                    equip.Option3 = r.GetInt16("option_3");
                    equip.Socket1 = r.GetInt16("socket_1");
                    equip.Socket2 = r.GetInt16("socket_1");
                    equip.TradeAvailable = r.GetInt32("trade_available");
                    equip.Vicious = r.GetByte("vicious");
                    equip.Upgrades = r.GetByte("upgrades");
                }
                equips.Clear();
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
        public ForcedStat ForcedStat { get; }
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
            
            using DatabaseQuery deleteEquips = Database.Table("equips");
            count = deleteEquips.Where("character_id", "=", CharacterStat.Id).Delete().ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Cleaned up {count} equips");

            using DatabaseQuery insertEquips = Database.Table("equips");
            using DatabaseQuery insertItems = Database.Table("items");
            foreach (var inventory in Inventories.Values) {
                foreach (var item in inventory.Items) {
                    insertItems.Insert(Database.CreateItemParameters(this, item));
                    if (item is ItemSlotEquip equip) {
                        insertEquips.Insert(Database.CreateEquipParameters(this, equip));
                    }
                }
            }

            count = insertItems.ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Saved {count} items");

            count = insertEquips.ExecuteNonQuery();
            Log.Info($"[Save] {CharacterStat.Username} : Saved {count} equips");
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
            Field?.BroadcastPacket(LeaveFieldPacket());
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
                CharacterStat.SendUpdate((uint) UserAbility.Money);
                return true;
            }

            return false;
        }
    }

    public class AvatarLook : IPacketSerializer {
        public AvatarLook(ref User user, MySqlDataReader r = null) {
            if (r == null) return;
            _user = user;
            Gender = r.GetByte("gender");
            Skin = r.GetByte("skin");
            Face = r.GetInt32("face");
            Hair = r.GetInt32("hair");
        }
        
        private readonly User _user;
        public byte Gender { get; set; }
        public byte Skin { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }

        public void Encode(Packet p) {
            p.WriteByte(_user.AvatarLook.Gender);
            p.WriteByte(_user.AvatarLook.Skin);
            p.WriteInt(_user.AvatarLook.Face);
            p.WriteByte();
            p.WriteInt(_user.AvatarLook.Hair);
            var inventory = _user.Inventories[InventoryType.Equipped];
            foreach (var item in inventory.Items.Where(i => i.BagIndex >= -99)) {
                p.WriteByte((byte) Math.Abs(item.BagIndex));
                p.WriteInt(item.TemplateId);
            }

            p.WriteByte(255);
            foreach (ItemSlot item in inventory.Items.Where(i => i.BagIndex <= -100)) {
                p.WriteByte((byte) item.BagIndex);
                p.WriteInt(item.TemplateId);
            }

            p.WriteByte(255);
            p.WriteInt(inventory[-11]?.TemplateId ?? 0);
            p.WriteBytes(new byte[12]); // pets
        }

        public void Decode(Packet p) {
            _user.AvatarLook.Gender = p.ReadByte();
            _user.AvatarLook.Skin = p.ReadByte();
            _user.AvatarLook.Face = p.ReadInt();
            _user.AvatarLook.Hair = p.ReadInt();
        }
    }

    public class CharacterStat : IPacketSerializer {
        private readonly User _user;
        private readonly short[] _skillPoints;
        private int _hp = 50, _mp = 5, _maxHP = 50, _maxMP = 5;
        private uint _exp;

        public CharacterStat(ref User user, MySqlDataReader r = null) {
            _skillPoints = new short[10];
            if (r == null) return;
            _user = user;
            Id = r.GetUInt32("character_id");
            Username = r.GetString("username");
            Level = r.GetByte("level");
            Job = r.GetInt16("job");
            Str = r.GetInt16("str");
            Dex = r.GetInt16("dex");
            Int = r.GetInt16("int");
            Luk = r.GetInt16("luk");
            MaxHP = r.GetInt32("max_hp");
            HP = r.GetInt32("hp");
            MaxMP = r.GetInt32("max_mp");
            MP = r.GetInt32("mp");
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


        public int MaxHP {
            get => _maxHP;
            set => _maxHP = Math.Min(Math.Max(value, 50), 99999);
        }

        public int HP {
            get => _hp;
            set => _hp = Math.Min(Math.Max(value, 0), MaxHP);
        }

        public int MaxMP {
            get => _maxMP;
            set => _maxMP = Math.Min(Math.Max(value, 5), 99999);
        }
        public int MP {
            get => _mp;
            set => _mp = Math.Min(Math.Max(value, 0), MaxMP);
        }

        public float Exp {
            get => _exp;
            set {
                float expGained = Math.Max(0, value - _exp);
                uint neededForLevel = GameConstants.GetExpToLevel(Level);
                uint neededToLevel = neededForLevel - Math.Min(_exp, neededForLevel - 1);
                if (expGained >= neededToLevel) {
                    while (expGained >= neededToLevel) {
                        expGained -= neededToLevel;
                        neededToLevel = GameConstants.GetExpToLevel(++Level);
                    }

                    _exp = (uint) expGained;
                } else {
                    _exp = (uint) (expGained + _exp);
                }
            }
        }
        
        public short AP { get; set; }

        public short SP {
            get {
                byte index = 0;
                if (JobConstants.IsExtendedSpJob(Job)) {
                    index = (byte) JobConstants.GetJobLevel(Job);
                }

                return _skillPoints[index];
            }
            set {
                byte index = 0;
                if (JobConstants.IsExtendedSpJob(Job)) {
                    index = (byte) JobConstants.GetJobLevel(Job);
                }

                _skillPoints[index] = value;
            }
        }

        public short[] SkillPoints => _skillPoints;
        public short Popularity { get; set; }
        public int FieldId { get; set; } = 10000;
        public byte Portal { get; set; }

        public void SendUpdate(uint dwcharFlags) {
            if (((UserAbility) dwcharFlags & UserAbility.HP) == UserAbility.HP) {
                if (_user.CharacterStat.HP < 1) {
                    _user.SendMessage("You have died.");
                }
            }

            _user.Client.Session.Write(CWvsPackets.GetStatChanged(_user, dwcharFlags));
        }

        public void Encode(Packet p) {
            if (Id == 0) throw new InvalidOperationException("cannot encode a character which id is 0");
            p.WriteUInt(Id);
            p.WriteStringFixed(Username, 13);
            p.WriteByte(_user.AvatarLook.Gender);
            p.WriteByte(_user.AvatarLook.Skin);
            p.WriteInt(_user.AvatarLook.Face);
            p.WriteInt(_user.AvatarLook.Hair);
            // pets
            p.WriteLong();
            p.WriteLong();
            p.WriteLong();

            p.WriteByte(_user.CharacterStat.Level);
            var jobId = p.WriteShort(_user.CharacterStat.Job);
            p.WriteShort(_user.CharacterStat.Str);
            p.WriteShort(_user.CharacterStat.Dex);
            p.WriteShort(_user.CharacterStat.Int);
            p.WriteShort(_user.CharacterStat.Luk);
            p.WriteInt(_user.CharacterStat.HP);
            p.WriteInt(_user.CharacterStat.MaxHP);
            p.WriteInt(_user.CharacterStat.MP);
            p.WriteInt(_user.CharacterStat.MaxMP);
            p.WriteShort(_user.CharacterStat.AP);

            if (JobConstants.IsExtendedSpJob(jobId)) {
                byte advancements = (byte) JobConstants.GetJobLevel(Job);
                p.WriteByte(advancements);
                for (byte i = 0; i < advancements; i++) {
                    p.WriteByte(i);
                    p.WriteByte((byte) SkillPoints[i]);
                }
            } else {
                p.WriteShort(SP);
            }

            p.WriteUInt((uint) Exp);
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
            if ((dwcharFlag & 0x10) == 0x10) p.WriteByte(user.CharacterStat.Level);
            if ((dwcharFlag & 0x20) == 0x20) p.WriteShort(user.CharacterStat.Job);
            if ((dwcharFlag & 0x40) == 0x40) p.WriteShort(user.CharacterStat.Str);
            if ((dwcharFlag & 0x80) == 0x80) p.WriteShort(user.CharacterStat.Dex);
            if ((dwcharFlag & 0x100) == 0x100) p.WriteShort(user.CharacterStat.Int);
            if ((dwcharFlag & 0x200) == 0x200) p.WriteShort(user.CharacterStat.Luk);
            if ((dwcharFlag & 0x400) == 0x400) p.WriteInt(user.CharacterStat.HP);
            if ((dwcharFlag & 0x800) == 0x800) p.WriteInt(user.CharacterStat.MaxHP);
            if ((dwcharFlag & 0x1000) == 0x1000) p.WriteInt(user.CharacterStat.MP);
            if ((dwcharFlag & 0x2000) == 0x2000) p.WriteInt(user.CharacterStat.MaxMP);
            if ((dwcharFlag & 0x4000) == 0x4000) p.WriteShort(user.CharacterStat.AP);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt((int) user.Money);

            if ((dwcharFlag & 0x8000) == 0x8000) {
                if (JobConstants.IsExtendedSpJob(user.CharacterStat.Job)) {
                    byte length = p.WriteByte((byte) user.CharacterStat.SkillPoints.Length);
                    for (byte i = 0; i < length; i++) {
                        p.WriteByte(i);
                        p.WriteByte((byte) user.CharacterStat.SkillPoints[i]);
                    }
                } else {
                    p.WriteShort(user.CharacterStat.SP);
                }
            }

            if ((dwcharFlag & 0x10000) == 0x10000) p.WriteInt((int) user.CharacterStat.Exp);
            if ((dwcharFlag & 0x20000) == 0x20000) p.WriteShort(user.CharacterStat.Popularity);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt();
            if ((dwcharFlag & 0x200000) == 0x200000) p.WriteInt(user.CharacterStat.FieldId);
        }

        public void Decode(Packet p) {
            _user.CharacterStat.Id = p.ReadUInt();
            _user.CharacterStat.Username = p.ReadString(13).Trim();
            _user.AvatarLook.Gender = p.ReadByte();
            _user.AvatarLook.Skin = p.ReadByte();
            _user.AvatarLook.Face = p.ReadInt();
            _user.AvatarLook.Hair = p.ReadInt();
            p.ReadLong();
            p.ReadLong();
            p.ReadLong();
            _user.CharacterStat.Level = p.ReadByte();
            var jobId = (_user.CharacterStat.Job = p.ReadShort());
            _user.CharacterStat.Str = p.ReadShort();
            _user.CharacterStat.Dex = p.ReadShort();
            _user.CharacterStat.Int = p.ReadShort();
            _user.CharacterStat.Luk = p.ReadShort();
            _user.CharacterStat._hp = p.ReadInt();
            _user.CharacterStat.MaxHP = p.ReadInt();
            _user.CharacterStat._mp = p.ReadInt();
            _user.CharacterStat.MaxMP = p.ReadInt();
            _user.CharacterStat.AP = p.ReadShort();

            if (JobConstants.IsExtendedSpJob(jobId)) {
                byte advancements = p.ReadByte();
                for (int i = 0; i < advancements; i++) {
                    _user.CharacterStat.SkillPoints[p.ReadByte()] = p.ReadByte();
                }
            } else {
                _user.CharacterStat.SP = p.ReadShort();
            }

            _user.CharacterStat.Exp = p.ReadUInt();
            _user.CharacterStat.Popularity = p.ReadShort();
            p.ReadInt();
            _user.CharacterStat.FieldId = p.ReadInt();
            _user.CharacterStat.Portal = p.ReadByte();
            p.ReadInt();
            p.ReadShort();
        }
    }
}