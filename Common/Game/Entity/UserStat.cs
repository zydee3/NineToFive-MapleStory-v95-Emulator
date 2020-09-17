using System;
using MySql.Data.MySqlClient;
using NineToFive.Constants;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.Util;

namespace NineToFive.Game.Entity {
    public class CharacterStat : IPacketSerializer {
        private readonly User _user;
        private readonly short[] _skillPoints;
        private uint _exp;

        private int _hp = 50;
        private int _mp = 50;

        private int _maxHP = 50;
        private int _maxMP = 50;
        private short _str = 4;
        private short _dex = 4;
        private short _int = 4;
        private short _luk = 4;

        private int _incMaxHP;
        private int _incMaxMP;
        private int _incSTR;
        private int _incDEX;
        private int _incINT;
        private int _incLUK;

        private int _incPAD;
        private int _incMAD;
        private int _incPDD;
        private int _incMDD;
        private int _incACC;
        private int _incEVA;
        private int _incCraft;
        private int _incSpeed;
        private int _incJump;

        private short _incPercentPAD;
        private short _incPercentMAD;
        private short _incPercentPDD;
        private short _incPercentMDD;
        private short _incPercentACC;
        private short _incPercentEVA;
        private short _incPercentCraft;
        private short _incPercentSpeed;
        private short _incPercentJump;

        /// <summary>
        /// Reduces assigning by 3 and comparisons by 2 compared to Math.Max(floor, Math.Min(value, ceil));
        /// </summary>
        /// <param name="stat">stat being changed and or bounded</param>
        /// <param name="value">value being assigned to stat</param>
        /// <param name="maxValue">max value stat can be</param>
        private void BoundValue(ref int stat, int value, int maxValue) {
            if (value > maxValue) stat = maxValue;
            else if (stat < 0) stat = 0;
            stat = value;
        }

        private void BoundValue(ref short stat, short value, short maxValue) {
            if (value > maxValue) stat = maxValue;
            else if (stat < 0) stat = 0;
            stat = value;
        }

        #region basic stats

        public int HP {
            get => _hp;
            set => BoundValue(ref _hp, value, MaxHP);
        }

        public int MP {
            get => _mp;
            set => BoundValue(ref _mp, value, MaxMP);
        }

        public int MaxHP {
            get => _maxHP;
            set => BoundValue(ref _maxHP, value, GameConstants.MaxHPMP);
        }

        public int MaxMP {
            get => _maxMP;
            set => BoundValue(ref _maxMP, value, GameConstants.MaxHPMP);
        }

        public short STR {
            get => _str;
            set => BoundValue(ref _str, value, GameConstants.MaxStat);
        }

        public short DEX {
            get => _dex;
            set => BoundValue(ref _dex, value, GameConstants.MaxStat);
        }

        public short INT {
            get => _int;
            set => BoundValue(ref _int, value, GameConstants.MaxStat);
        }

        public short LUK {
            get => _luk;
            set => BoundValue(ref _luk, value, GameConstants.MaxStat);
        }

        #endregion

        #region inc stats

        public int IncMaxHP {
            get => _incMaxHP;
            set => BoundValue(ref _incMaxHP, value, int.MaxValue);
        }

        public int IncMaxMP {
            get => _incMaxMP;
            set => BoundValue(ref _incMaxMP, value, int.MaxValue);
        }

        public int IncSTR {
            get => _incSTR;
            set => BoundValue(ref _incSTR, value, int.MaxValue);
        }

        public int IncDEX {
            get => _incDEX;
            set => BoundValue(ref _incDEX, value, int.MaxValue);
        }

        public int IncINT {
            get => _incINT;
            set => BoundValue(ref _incINT, value, int.MaxValue);
        }

        public int IncLUK {
            get => _incLUK;
            set => BoundValue(ref _incLUK, value, int.MaxValue);
        }

        public int IncPAD {
            get => _incPDD;
            set => BoundValue(ref _incPAD, value, int.MaxValue);
        }

        public int IncMAD {
            get => _incMDD;
            set => BoundValue(ref _incMAD, value, int.MaxValue);
        }

        public int IncPDD {
            get => _incPDD;
            set => BoundValue(ref _incPDD, value, int.MaxValue);
        }

        public int IncMDD {
            get => _incMDD;
            set => BoundValue(ref _incMDD, value, int.MaxValue);
        }

        public int IncACC {
            get => _incACC;
            set => BoundValue(ref _incACC, value, int.MaxValue);
        }

        public int IncEVA {
            get => _incEVA;
            set => BoundValue(ref _incEVA, value, int.MaxValue);
        }

        public int IncCraft {
            get => _incCraft;
            set => BoundValue(ref _incCraft, value, int.MaxValue);
        }

        public int IncSpeed {
            get => _incSpeed;
            set => BoundValue(ref _incSpeed, value, int.MaxValue);
        }

        public int IncJump {
            get => _incJump;
            set => BoundValue(ref _incJump, value, int.MaxValue);
        }

        public short IncPercentPAD {
            get => _incPercentPDD;
            set => BoundValue(ref _incPercentPAD, value, short.MaxValue);
        }

        public short IncPercentMAD {
            get => _incPercentMDD;
            set => BoundValue(ref _incPercentMAD, value, short.MaxValue);
        }

        public short IncPercentPDD {
            get => _incPercentPDD;
            set => BoundValue(ref _incPercentPDD, value, short.MaxValue);
        }

        public short IncPercentMDD {
            get => _incPercentMDD;
            set => BoundValue(ref _incPercentMDD, value, short.MaxValue);
        }

        public short IncPercentACC {
            get => _incPercentACC;
            set => BoundValue(ref _incPercentACC, value, short.MaxValue);
        }

        public short IncPercentEVA {
            get => _incPercentEVA;
            set => BoundValue(ref _incPercentEVA, value, short.MaxValue);
        }

        public short IncPercentCraft {
            get => _incPercentCraft;
            set => BoundValue(ref _incPercentCraft, value, short.MaxValue);
        }

        public short IncPercentSpeed {
            get => _incPercentSpeed;
            set => BoundValue(ref _incPercentSpeed, value, short.MaxValue);
        }

        public short IncPercentJump {
            get => _incPercentJump;
            set => BoundValue(ref _incPercentJump, value, short.MaxValue);
        }

        #endregion

        #region total stats

        public int TotalMaxHP => Math.Min(GameConstants.MaxHPMP, _maxHP + _incMaxHP);
        public int TotalMaxMP => Math.Min(GameConstants.MaxHPMP, _maxMP + _incMaxMP);
        public short TotalMaxSTR => (short) Math.Min(GameConstants.MaxStat, _str + _incSTR);
        public short TotalMaxDEX => (short) Math.Min(GameConstants.MaxStat, _dex + _incDEX);
        public short TotalMaxINT => (short) Math.Min(GameConstants.MaxStat, _int + _incINT);
        public short TotalMaxLUK => (short) Math.Min(GameConstants.MaxStat, _luk + _incLUK);
        public int TotalPAD => Math.Min(int.MaxValue, _incPAD + _incPAD * _incPercentPAD);
        public int TotalMAD => Math.Min(int.MaxValue, _incMAD + _incMAD * _incPercentMAD);
        public int TotalPDD => Math.Min(int.MaxValue, _incPDD + _incPDD * _incPercentPDD);
        public int TotalMDD => Math.Min(int.MaxValue, _incMDD + _incMDD * _incPercentMDD);
        public int TotalACC => Math.Min(int.MaxValue, _incACC + _incACC * _incPercentACC);
        public int TotalEVA => Math.Min(int.MaxValue, _incEVA + _incEVA * _incPercentEVA);
        public int TotalCraft => Math.Min(int.MaxValue, _incCraft + _incCraft * _incPercentCraft);
        public int TotalSpeed => Math.Min(int.MaxValue, _incSpeed + _incSpeed * _incPercentSpeed);
        public int TotalJump => Math.Min(int.MaxValue, _incJump + _incJump * _incPercentJump);

        #endregion

        public uint Id { get; set; }
        public string Username { get; set; }
        public byte Level { get; set; } = 1;
        public short Job { get; set; }
        public short AP { get; set; }


        public short[] SkillPoints => _skillPoints;
        public short Popularity { get; set; }
        public int FieldId { get; set; } = 10000;
        public byte Portal { get; set; }

        public CharacterStat(ref User user, MySqlDataReader r = null) {
            _skillPoints = new short[10];
            if (r == null) return;
            _user = user;
            Id = r.GetUInt32("character_id");
            Username = r.GetString("username");
            Level = r.GetByte("level");
            Job = r.GetInt16("job");
            STR = r.GetInt16("str");
            DEX = r.GetInt16("dex");
            INT = r.GetInt16("int");
            LUK = r.GetInt16("luk");
            MaxHP = r.GetInt32("max_hp");
            HP = r.GetInt32("hp");
            MaxMP = r.GetInt32("max_mp");
            MP = r.GetInt32("mp");
            AP = r.GetInt16("ability_points");
            Exp = r.GetUInt32("exp");
            Popularity = r.GetInt16("popularity");
            FieldId = r.GetInt32("field_id");
            Portal = r.GetByte("portal");
            Console.WriteLine($"{r.GetInt16("str")}, {r.GetInt16("dex")}, {r.GetInt16("int")}, {r.GetInt16("luk")}");
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

        public void SendUpdate(uint dwcharFlags) {
            if (((UserAbility) dwcharFlags & UserAbility.HP) == UserAbility.HP) {
                if (_user.CharacterStat.HP < 1) {
                    _user.SendMessage("You have died.");
                }
            }

            _user.Client.Session.Write(CWvsPackets.GetStatChanged(_user, dwcharFlags));
        }

        public void ResetIncStats() {
            _incSTR = 0;
            _incDEX = 0;
            _incINT = 0;
            _incLUK = 0;
            _incMaxHP = 0;
            _incMaxMP = 0;
            _incPAD = 0;
            _incMAD = 0;
            _incPDD = 0;
            _incMDD = 0;
            _incACC = 0;
            _incEVA = 0;
            _incCraft = 0;
            _incSpeed = 0;
            _incJump = 0;

            _incPercentPAD = 0;
            _incPercentMAD = 0;
            _incPercentPDD = 0;
            _incPercentMDD = 0;
            _incPercentACC = 0;
            _incPercentEVA = 0;
            _incPercentCraft = 0;
            _incPercentSpeed = 0;
            _incPercentSpeed = 0;
            _incPercentJump = 0;
        }

        /// <summary>
        /// Re-evaluates the raw and percent stat increases from stats and buffs.
        /// </summary>
        /// <param name="updateEquipIncStat">Whether to re-calculate the base stats or not. Should be true when an equipped item gets changed and false
        /// otherwise for instances like AbilityUpEvent</param>
        public void UpdateIncStats(bool updateEquipIncStat = true) {
            ResetIncStats();
            var equipInventory = _user.Inventories[InventoryType.Equipped].Items;

            if (updateEquipIncStat) {
                foreach (ItemSlot item in equipInventory) {
                    GainEquipIncStat((ItemSlotEquip) item);
                }
            }

            //todo buff base stat
            
            foreach (ItemSlot item in equipInventory) {
                GainEquipIncStatPercent((ItemSlotEquip) item);
            }

            //todo buff stat percentages
        }

        public void GainEquipIncStat(ItemSlotEquip equip, bool equipping = true) {
            int multiple = equipping ? 1 : -1;
            IncMaxHP += equip.MaxMP * multiple;
            IncMaxMP += equip.MaxMP * multiple;
            IncSTR += equip.STR * multiple;
            IncDEX += equip.DEX * multiple;
            IncINT += equip.INT * multiple;
            IncLUK += equip.LUK * multiple;
            IncPAD += equip.PAD * multiple;
            IncMAD += equip.MAD * multiple;
            IncPDD += equip.PDD * multiple;
            IncMDD += equip.MDD * multiple;
            IncACC += equip.ACC * multiple;
            IncEVA += equip.EVA * multiple;
            IncCraft += equip.Craft * multiple;
            IncSpeed += equip.Speed * multiple;
            IncJump += equip.Jump * multiple;
        }

        public void GainEquipIncStatPercent(ItemSlotEquip equip, bool equipping = true) {
            int multiple = equipping ? 1 : -1;
            IncMaxHP += equip.MaxHPR * MaxHP * multiple;
            IncMaxMP += equip.MaxMPR * MaxMP * multiple;

            //todo calculate percentages from potentials
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

            p.WriteByte(Level);
            var jobId = p.WriteShort(Job);
            p.WriteShort(TotalMaxSTR);
            p.WriteShort(TotalMaxDEX);
            p.WriteShort(TotalMaxINT);
            p.WriteShort(TotalMaxLUK);
            p.WriteInt(HP);
            p.WriteInt(TotalMaxHP);
            p.WriteInt(MP);
            p.WriteInt(TotalMaxMP);
            p.WriteShort(AP);

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
            if ((dwcharFlag & 8) == 8) p.WriteLong(); // pet 1
            if ((dwcharFlag & 0x80000) == 0x80000) p.WriteLong(); // pet 2
            if ((dwcharFlag & 0x100000) == 0x100000) p.WriteLong(); // pet 3
            if ((dwcharFlag & 0x10) == 0x10) p.WriteByte(Level);
            if ((dwcharFlag & 0x20) == 0x20) p.WriteShort(Job);
            if ((dwcharFlag & 0x40) == 0x40) p.WriteShort(TotalMaxSTR);
            if ((dwcharFlag & 0x80) == 0x80) p.WriteShort(TotalMaxDEX);
            if ((dwcharFlag & 0x100) == 0x100) p.WriteShort(TotalMaxINT);
            if ((dwcharFlag & 0x200) == 0x200) p.WriteShort(TotalMaxLUK);
            if ((dwcharFlag & 0x400) == 0x400) p.WriteInt(HP);
            if ((dwcharFlag & 0x800) == 0x800) p.WriteInt(TotalMaxHP);
            if ((dwcharFlag & 0x1000) == 0x1000) p.WriteInt(MP);
            if ((dwcharFlag & 0x2000) == 0x2000) p.WriteInt(TotalMaxMP);
            if ((dwcharFlag & 0x4000) == 0x4000) p.WriteShort(AP);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt((int) user.Money);

            if ((dwcharFlag & 0x8000) == 0x8000) {
                if (JobConstants.IsExtendedSpJob(Job)) {
                    byte length = p.WriteByte((byte) SkillPoints.Length);
                    for (byte i = 0; i < length; i++) {
                        p.WriteByte(i);
                        p.WriteByte((byte) SkillPoints[i]);
                    }
                } else {
                    p.WriteShort(SP);
                }
            }

            if ((dwcharFlag & 0x10000) == 0x10000) p.WriteInt((int) Exp);
            if ((dwcharFlag & 0x20000) == 0x20000) p.WriteShort(Popularity);
            if ((dwcharFlag & 0x40000) == 0x40000) p.WriteInt();
            if ((dwcharFlag & 0x200000) == 0x200000) p.WriteInt(FieldId);
        }

        public void Decode(Packet p) {
            Id = p.ReadUInt();
            Username = p.ReadString(13).Trim();
            _user.AvatarLook.Gender = p.ReadByte();
            _user.AvatarLook.Skin = p.ReadByte();
            _user.AvatarLook.Face = p.ReadInt();
            _user.AvatarLook.Hair = p.ReadInt();
            p.ReadLong();
            p.ReadLong();
            p.ReadLong();
            _user.CharacterStat.Level = p.ReadByte();
            var jobId = (_user.CharacterStat.Job = p.ReadShort());
            STR = p.ReadShort();
            DEX = p.ReadShort();
            INT = p.ReadShort();
            LUK = p.ReadShort();
            HP = p.ReadInt();
            MaxHP = p.ReadInt();
            MP = p.ReadInt();
            MaxMP = p.ReadInt();
            AP = p.ReadShort();

            if (JobConstants.IsExtendedSpJob(jobId)) {
                byte advancements = p.ReadByte();
                for (int i = 0; i < advancements; i++) {
                    SkillPoints[p.ReadByte()] = p.ReadByte();
                }
            } else {
                SP = p.ReadShort();
            }

            Exp = p.ReadUInt();
            Popularity = p.ReadShort();
            p.ReadInt();
            FieldId = p.ReadInt();
            Portal = p.ReadByte();
            p.ReadInt();
            p.ReadShort();
        }
    }
}