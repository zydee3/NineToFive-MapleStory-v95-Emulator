namespace NineToFive.Game.Entity.Meta {
    public class TemplateEquip {
        public TemplateEquip(int itemId) {
            ItemId = itemId;
        }

        public int ItemId { get; }

        #region Increase stats

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

        #endregion

        public short AttackSpeed { get; set; }
        public short HPRecovery { get; set; }
        public short MPRecovery { get; set; }
        public short KnockBack { get; set; }
        public short Walk { get; set; }
        public short Stand { get; set; }
        public short Speed { get; set; }

        #region Require stats

        public short ReqLevel { get; set; }
        public short ReqJob { get; set; }
        public short ReqSTR { get; set; }
        public short ReqDEX { get; set; }
        public short ReqINT { get; set; }
        public short ReqLUK { get; set; }
        public short ReqPOP { get; set; }

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
        public int Durability { get; set; } = -1;
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
        
        public int ConsumeHP { get; set; }
        public int ConsumeMP { get; set; }
        
        public string VSlot { get; set; }
        public string ISlot { get; set; }
        public string AfterImage { get; set; }
        public string Sfx { get; set; }
        public string PickupMeso { get; set; }
        public string PickupItem { get; set; }
        public string PickupOthers { get; set; }
        public string SweepForDrop { get; set; }
        public string LongRange { get; set; }

        public float Recovery { get; set; }
        public short Attack { get; set; }
    }
}