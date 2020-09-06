using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
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
        
        public string Tag { get; set; }
        public short Attribute { get; set; }
        
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
        
        public short Hp { get; set; }
        public short Mp { get; set; }
        public short Pad { get; set; }
        public short Mad { get; set; }
        public short Prob { get; set; }
        public short Eva { get; set; }
        public string DefenseAtt { get; set; }
        public short Jump { get; set; }
        public short Acc { get; set; }
        public short Str { get; set; }
        public short Luk { get; set; }
        public short Int { get; set; }
        public short Dex { get; set; }
        public short Pdd { get; set; }
        public short Mdd { get; set; }
        public short Speed { get; set; }
        
        #endregion
        #region Increase
        
        public short IncPERIOD { get; set; }
        public short IncPAD { get; set; }
        public short IncMDD { get; set; }
        public short IncACC { get; set; }
        public short IncMHP { get; set; }
        public short Cursed { get; set; }
        public short IncINT { get; set; }
        public short IncDEX { get; set; }
        public short IncMAD { get; set; }
        public short IncEVA { get; set; }
        public short IncSTR { get; set; }
        public short IncLUK { get; set; }
        public short IncSpeed { get; set; }
        public short IncMMP { get; set; }
        public short IncJump { get; set; }
        public short Inc { get; set; }
        public short IncIUC { get; set; }
        public short IncCraft { get; set; }
        public short IncRandVol { get; set; }
        public short Expinc { get; set; }
        public short IncLEV { get; set; }
        public short IncFatigue { get; set; }
        public short IncMaxHP { get; set; }
        public short IncMaxMP { get; set; }
        public short IncReqLevel { get; set; }
        
        #endregion
        #region Require
        
        public short ReqLevel { get; set; }
        public short ReqCUC { get; set; }
        public short ReqRUC { get; set; }
        
        #endregion
        #region Rate
        
        public short PadRate { get; set; }
        public short MadRate { get; set; }
        public short PddRate { get; set; }
        public short MddRate { get; set; }
        public short AccRate { get; set; }
        public short EvaRate { get; set; }
        public short SpeedRate { get; set; }
        public short MhpRRate { get; set; }
        public short MmpRRate { get; set; }
        public short HpR { get; set; }
        public short MpR { get; set; }
        public short MmpR { get; set; }
        public short MhpR { get; set; }
        
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

        public bool IsRechargable {
            get {
                int category = Id / 10000;
                return category == 207 || category == 233;
            }
        }

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

        public void ApplyToUser(User user, uint flag = 0) {
            CharacterStat stats = user.CharacterStat;
            Parallel.ForEach((ItemSpec[]) Enum.GetValues(typeof(ItemSpec)), spec => DoUserStatChange(spec,ref stats, ref flag));
            user.CharacterStat.SendUpdate(user, flag);
        }

        private void DoUserStatChange(ItemSpec spec, ref CharacterStat stats, ref uint flag) {
            switch (spec) {
                case ItemSpec.Hp when Hp > 0:
                    stats.HP += Hp;
                    flag |= (uint) UserAbility.HP;
                    break;
                case ItemSpec.HpR when HpR > 0:
                    stats.HP += (int) (stats.MaxHP * (HpR * 1.0 / 100));
                    flag |= (uint) UserAbility.HP;
                    break;
                case ItemSpec.Mp when Mp > 0:
                    stats.MP += Mp;
                    flag |= (uint) UserAbility.MP;
                    break;
                case ItemSpec.MpR when MpR > 0:
                    stats.MP += (int) (stats.MaxMP * (MpR * 1.0 / 100));
                    flag |= (uint) UserAbility.MP;
                    break;
                
                case ItemSpec.Str when Str > 0:
                    stats.Str += Str;
                    flag |= (uint) UserAbility.Str;
                    break;
                case ItemSpec.Luk when Luk > 0:
                    stats.Luk += Luk;
                    flag |= (uint) UserAbility.Luk;
                    break;
                case ItemSpec.Int when Int > 0:
                    stats.Int += Int;
                    flag |= (uint) UserAbility.Int;
                    break;
                case ItemSpec.Dex when Dex > 0:
                    stats.Dex += Dex;
                    flag |= (uint) UserAbility.Dex;
                    break;
                
                //todo add rest of ItemSpec values
            }
        }
    }
}