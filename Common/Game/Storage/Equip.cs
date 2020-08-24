using System;
using NineToFive.Net;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class Equip : Item {
        /// <param name="id">the id if the equip</param>
        /// <param name="autoBagIndex">should the BagIndex be automatically assigned to the associated body part</param>
        /// <param name="loadStats">should default stats for the item be loaded</param>
        public Equip(int id, bool autoBagIndex = false, bool loadStats = false) : base(id) {
            if (InventoryType != InventoryType.Equip) throw new InvalidOperationException("cannot create Equip of Item : " + id);
            if (autoBagIndex) BagIndex = (short) -ItemConstants.GetBodyPartFromId(id);
            if (loadStats) CharacterWz.CopyEquipTemplate(this);
        }

        public override string ToString() {
            return $"Equip{{ID: {Id}, BagIndex: {BagIndex}}}";
        }

        public override byte Type => 1;

        public override ushort Quantity => 1;

        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public short MaxHP { get; set; }
        public short MaxMP { get; set; }
        public short PAD { get; set; }
        public short MAD { get; set; }
        public short PDD { get; set; }
        public short MDD { get; set; }
        public short Acc { get; set; }
        public short Eva { get; set; }
        public short Craft { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public string Title { get; set; } = "";
        public byte CHUC { get; set; }

        public int EquipId { get; set; }

        public short IncHP { get; set; }
        public short IncMHP { get; set; }
        public short IncMP { get; set; }
        public short IncMMP { get; set; }
        public short IncSTR { get; set; }
        public short IncDEX { get; set; }
        public short IncINT { get; set; }
        public short IncLUK { get; set; }
        public short IncPAD { get; set; }
        public short IncMAD { get; set; }
        public short IncPDD { get; set; }
        public short IncMDD { get; set; }
        public short IncACC { get; set; }
        public short IncEVA { get; set; }
        public short IncSpeed { get; set; }
        public short IncJump { get; set; }
        public short IncRMAF { get; set; }
        public short IncRMAS { get; set; }
        public short IncRMAI { get; set; }
        public short IncRMAL { get; set; }
        public short IncCraft { get; set; }
        public short IncMHPR { get; set; }
        public short IncMMPR { get; set; }
        public short IncSwim { get; set; }
        public short IncFatigue { get; set; }

        public short AttackSpeed { get; set; }
        public short HPRecovery { get; set; }
        public short MPRecovery { get; set; }
        public short KnockBack { get; set; }
        public short Walk { get; set; }
        public short Stand { get; set; }

        public short ReqLevel { get; set; }
        public short ReqJob { get; set; }
        public short ReqSTR { get; set; }
        public short ReqDEX { get; set; }
        public short ReqINT { get; set; }
        public short ReqLUK { get; set; }
        public short ReqPOP { get; set; }

        public int Price { get; set; }
        public int NotSale { get; set; }
        public int TradeBlock { get; set; }
        public int EquipTradeBlock { get; set; }
        public int AccountSharable { get; set; }
        public int DropBlock { get; set; }
        public int SlotMax { get; set; }
        public int TimeLimited { get; set; }
        public int TradeAvailable { get; set; }
        public int ExpireOnLogout { get; set; }
        public int NotExtend { get; set; }

        public int OnlyEquip { get; set; }
        public int Pachinko { get; set; }
        public int ChatBalloon { get; set; }
        public int NameTag { get; set; }
        public int SharableOnce { get; set; }
        public int TamingMob { get; set; }
        public int TUC { get; set; }
        public int Cash { get; set; }
        public int IgnorePickup { get; set; }
        public int SetItemID { get; set; }
        public int Only { get; set; }
        public int Durability { get; set; }
        public int ElemDefault { get; set; }
        public int ScanTradeBlock { get; set; }
        public int EpicItem { get; set; }
        public int Hide { get; set; }
        public int Quest { get; set; }
        public int Weekly { get; set; }
        public int EnchantCategory { get; set; }
        public int IUCMax { get; set; }
        public int Fs { get; set; }
        public int MedalTag { get; set; }
        public int NoExpend { get; set; }
        public int SpecialID { get; set; }

        public string VSlot { get; set; }
        public string ISlot { get; set; }
        public string AfterImage { get; set; }
        public string Sfx { get; set; }
        public string PickupMeso { get; set; }
        public string PickupItem { get; set; }
        public string PickupOthers { get; set; }
        public string SweepForDrop { get; set; }
        public string ConsumeHP { get; set; }
        public string LongRange { get; set; }
        public string ConsumeMP { get; set; }

        public float Recovery { get; set; }
        public short Attack { get; set; }

        public override void Encode(Item item, Packet p) {
            if (!(item is Equip equip)) throw new NullReferenceException();
            base.Encode(equip, p);
            p.WriteByte(); // nRUC
            p.WriteByte(); // nCUC
            p.WriteShort((short)equip.IncSTR);
            p.WriteShort((short)equip.IncDEX);
            p.WriteShort((short)equip.IncINT);
            p.WriteShort((short)equip.IncLUK);
            p.WriteShort((short)equip.IncMHP);
            p.WriteShort((short)equip.IncMMP);
            p.WriteShort((short)equip.IncPAD);
            p.WriteShort((short)equip.IncMAD);
            p.WriteShort((short)equip.IncPDD);
            p.WriteShort((short)equip.IncMDD);
            p.WriteShort((short)equip.IncACC);
            p.WriteShort((short)equip.IncEVA);

            p.WriteShort(equip.Craft);
            p.WriteShort(equip.Speed);
            p.WriteShort(equip.Jump);
            p.WriteString(equip.Title);
            p.WriteShort();

            p.WriteByte(); // iLevelUpType
            p.WriteByte(); // iLevel
            p.WriteInt();  // iEXP
            p.WriteInt(equip.Durability);  // iDurability

            p.WriteInt(); // nIUC

            p.WriteByte(); // nGrade
            p.WriteByte(equip.CHUC);
            p.WriteShort(); // nOption1
            p.WriteShort(); // nOption2
            p.WriteShort(); // nOption3
            p.WriteShort(); // nOption4/Socket1
            p.WriteShort(); // nOption5/Socket2
            if (equip.CashItemSn == 0) {
                p.WriteLong(equip.CashItemSn);
            }

            p.WriteLong(); // ftEquipped (file time?)
            p.WriteInt();  // nPrevBonusExpRate
        }

        public override void Decode(Item item, Packet p) { }
    }
}