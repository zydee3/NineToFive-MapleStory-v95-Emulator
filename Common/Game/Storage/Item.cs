using System;
using NineToFive.Net;
using NineToFive.Util;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class Item : IPacketSerializer<Item> {
        public Item(int id, bool setItem = false) {
            Id = id;
            InventoryType = ItemConstants.GetInventoryType(id);

            if (setItem) ItemWz.SetItem(this);
            
            //todo need to load item from database
        }

        public override string ToString() {
            return $"Item{{ID: {Id}, BagIndex: {BagIndex}}}";
        }

        public virtual byte Type => 2;
        public InventoryType InventoryType { get; }
        public int Id { get; }
        public uint GeneratedId { get; set; }
        public short BagIndex { get; set; }
        public virtual ushort Quantity { get; set; }
        public long CashItemSn { get; set; } // FILETIME
        public long DateExpire { get; set; }
        
        public int SlotMax { get; set; }
        public int Price { get; set; }
        public double UnitPrice { get; set; }
        public int PickUpBlock { get; set; }
        public int TradeBlock { get; set; }
        public int ConsumeOnPickup { get; set; }
        public int NoCancelMouse { get; set; }
        public int ExpireOnLogout { get; set; }
        public int AccountSharable { get; set; }
        public int Quest { get; set; }
        public int Only { get; set; }
        
        public int EnchantCategory { get; set; }
        public int Success { get; set; }
        public int NotSale { get; set; }
        public int TimeLimited { get; set; }
        public int Morph { get; set; }
        public int MasterLevel { get; set; }
        public int Time { get; set; }
        
        #region Stats
        
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Pad { get; set; }
        public int Mad { get; set; }
        public int Prob { get; set; }
        public int Eva { get; set; }
        public string DefenseAtt { get; set; }
        public int Jump { get; set; }
        public int Acc { get; set; }
        public int Str { get; set; }
        public int Luk { get; set; }
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Pdd { get; set; }
        public int Mdd { get; set; }
        public int Speed { get; set; }
        
        #endregion
        #region Increase
        
        public int IncPERIOD { get; set; }
        public int IncPAD { get; set; }
        public int IncMDD { get; set; }
        public int IncACC { get; set; }
        public int IncMHP { get; set; }
        public int Cursed { get; set; }
        public int IncINT { get; set; }
        public int IncDEX { get; set; }
        public int IncMAD { get; set; }
        public int IncEVA { get; set; }
        public int IncSTR { get; set; }
        public int IncLUK { get; set; }
        public int IncSpeed { get; set; }
        public int IncMMP { get; set; }
        public int IncJump { get; set; }
        public int Inc { get; set; }
        public int IncIUC { get; set; }
        public int IncCraft { get; set; }
        public int IncRandVol { get; set; }
        public int Expinc { get; set; }
        public int IncLEV { get; set; }
        public int IncFatigue { get; set; }
        public int IncMaxHP { get; set; }
        public int IncMaxMP { get; set; }
        public int IncReqLevel { get; set; }
        
        #endregion
        #region Require
        
        public int ReqLevel { get; set; }
        public int ReqCUC { get; set; }
        public int ReqRUC { get; set; }
        
        #endregion
        #region Rate
        
        public int PadRate { get; set; }
        public int MadRate { get; set; }
        public int PddRate { get; set; }
        public int MddRate { get; set; }
        public int AccRate { get; set; }
        public int EvaRate { get; set; }
        public int SpeedRate { get; set; }
        public int MhpRRate { get; set; }
        public int MmpRRate { get; set; }
        public int HpR { get; set; }
        public int MpR { get; set; }
        public int MmpR { get; set; }
        public int MhpR { get; set; }
        
        #endregion
        #region States
        
        public int Poison { get; set; }
        public int Darkness { get; set; }
        public int Weakness { get; set; }
        public int Seal { get; set; }
        public int Curse { get; set; }
        
        #endregion
        
        public int MaxLevel { get; set; }
        public int Exp { get; set; }
        public int MoveTo { get; set; }
        public int MaxDays { get; set; }
        public int QuestId { get; set; }
        
        public int RecoveryHP { get; set; }
        public int RecoveryMP { get; set; }
        public int ConsumeHP { get; set; }
        public int ConsumeMP { get; set; }

        public virtual void Encode(Item item, Packet p) {
            p.WriteByte(item.Type);
            p.WriteInt(item.Id);
            if (p.WriteBool(item.CashItemSn > 0)) {
                // liCashItemSN->low
                // liCashItemSN->high
                p.WriteLong(item.CashItemSn);
            }

            // p.WriteLong(DateTime.FromFileTimeUtc(150842304000000000).ToFileTime()); // No expiration
            p.WriteLong(item.DateExpire);
        }

        public virtual void Decode(Item item, Packet p) { }
    }
}