using System;
using NineToFive.Net;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class Equip : Item {
        /// <param name="id">the id if the equip</param>
        /// <param name="autoBagIndex">should the BagIndex be automatically assigned to the associated body part</param>
        public Equip(int id, bool autoBagIndex = false, bool setEquip = false) : base(id) {
            if (InventoryType != InventoryType.Equip) throw new InvalidOperationException("cannot create Equip of Item : " + id);
            if (autoBagIndex) BagIndex = (short) -ItemConstants.GetBodyPartFromId(id);
            if (setEquip) CharacterWz.CopyEquipTemplate(this);
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
        
        public int IncHP { get; set; }
        public int IncMHP { get; set; }
        public int IncMP { get; set; }
        public int IncMMP { get; set; }
        public int IncSTR { get; set; }
        public int IncDEX { get; set; }
        public int IncINT { get; set; }
        public int IncLUK { get; set; }
        public int IncPAD { get; set; }
        public int IncMAD { get; set; }
        public int IncPDD { get; set; }
        public int IncMDD { get; set; }
        public int IncACC { get; set; }
        public int IncEVA { get; set; }
        public int IncSpeed { get; set; }
        public int IncJump { get; set; }
        public int IncRMAF { get; set; }
        public int IncRMAS { get; set; }
        public int IncRMAI { get; set; }
        public int IncRMAL { get; set; }
        public int IncCraft { get; set; }
        public int IncMHPR { get; set; }
        public int IncMMPR { get; set; }
        public int IncSwim { get; set; }
        public int IncFatigue { get; set; }
        
        public int AttackSpeed { get; set; }
        public int HPRecovery { get; set; }
        public int MPRecovery { get; set; }
        public int KnockBack { get; set; }
        public int Walk { get; set; }
        public int Stand { get; set; }
        
        public int ReqLevel { get; set; }
        public int ReqJob { get; set; }
        public int ReqSTR { get; set; }
        public int ReqDEX { get; set; }
        public int ReqINT { get; set; }
        public int ReqLUK { get; set; }
        public int ReqPOP { get; set; }
        
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
            p.WriteShort(equip.Str);
            p.WriteShort(equip.Dex);
            p.WriteShort(equip.Int);
            p.WriteShort(equip.Luk);
            p.WriteShort(equip.MaxHP);
            p.WriteShort(equip.MaxMP);
            p.WriteShort(equip.PAD);
            p.WriteShort(equip.MAD);
            p.WriteShort(equip.PDD);
            p.WriteShort(equip.MDD);
            p.WriteShort(equip.Acc);
            p.WriteShort(equip.Eva);

            p.WriteShort(equip.Craft);
            p.WriteShort(equip.Speed);
            p.WriteShort(equip.Jump);
            p.WriteString(equip.Title);
            p.WriteShort();

            p.WriteByte(); // iLevelUpType
            p.WriteByte(); // iLevel
            p.WriteInt();  // iEXP
            p.WriteInt();  // iDurability

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