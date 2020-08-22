namespace NineToFive.Game.Entity.Meta {
    public class TemplateEquip {
        public TemplateEquip(int itemId) {
            ItemId = itemId;
        }

        public int ItemId { get; }

        #region Increase stats

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

        #endregion

        public int AttackSpeed { get; set; }
        public int HPRecovery { get; set; }
        public int MPRecovery { get; set; }
        public int KnockBack { get; set; }
        public int Walk { get; set; }
        public int Stand { get; set; }
        public int Speed { get; set; }

        #region Require stats

        public int ReqLevel { get; set; }
        public int ReqJob { get; set; }
        public int ReqSTR { get; set; }
        public int ReqDEX { get; set; }
        public int ReqINT { get; set; }
        public int ReqLUK { get; set; }
        public int ReqPOP { get; set; }

        #endregion

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
    }
}