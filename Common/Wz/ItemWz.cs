using System;
using System.Collections.Generic;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Game.Storage;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public class ItemWz {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemWz));

        /* todo these
          successRates (MapleLib.WzLib.WzProperties.WzSubProperty) => Consume/0204.img/02047000/info,
           cursedRates (MapleLib.WzLib.WzProperties.WzSubProperty) => Consume/0204.img/02047000/info,
             successes (MapleLib.WzLib.WzProperties.WzSubProperty) => Consume/0204.img/02047101/info,
                reqMap (MapleLib.WzLib.WzProperties.WzSubProperty) => Consume/0210.img/02100067/info
                 skill (MapleLib.WzLib.WzProperties.WzSubProperty) => Consume/0228.img/02280000/info,
         */
        public static void SetItem(Item item) {
            if (item == null) return;

            if (!WzCache.Items.TryGetValue(item.Id, out Item t)) {
                InitializeItem(out t, item.Id);
                WzCache.Items.Add(item.Id, t);
            }

            item.SlotMax = t.SlotMax;
            item.Price = t.Price;
            item.UnitPrice = t.UnitPrice;
            item.PickUpBlock = t.PickUpBlock;
            item.TradeBlock = t.TradeBlock;
            item.MaxLevel = t.MaxLevel;
            item.Exp = t.Exp;
            item.MoveTo = t.MoveTo;
            item.MaxDays = t.MaxDays;
            item.QuestId = t.QuestId;
            item.NoCancelMouse = t.NoCancelMouse;
            item.ExpireOnLogout = t.ExpireOnLogout;
            item.AccountSharable = t.AccountSharable;
            item.Quest = t.Quest;
            item.Only = t.Only;
            item.ConsumeOnPickup = t.ConsumeOnPickup;
            item.Hp = t.Hp;
            item.Mp = t.Mp;
            item.Pad = t.Pad;
            item.Mad = t.Mad;
            item.Prob = t.Prob;
            item.Eva = t.Eva;
            item.DefenseAtt = t.DefenseAtt;
            item.Jump = t.Jump;
            item.Acc = t.Acc;
            item.Str = t.Str;
            item.Luk = t.Luk;
            item.Int = t.Int;
            item.Dex = t.Dex;
            item.Pdd = t.Pdd;
            item.Mdd = t.Mdd;
            item.Speed = t.Speed;
            item.EnchantCategory = t.EnchantCategory;
            item.Success = t.Success;
            item.NotSale = t.NotSale;
            item.TimeLimited = t.TimeLimited;
            item.Morph = t.Morph;
            item.MasterLevel = t.MasterLevel;
            item.Time = t.Time;
            item.IncPERIOD = t.IncPERIOD;
            item.IncPAD = t.IncPAD;
            item.IncMDD = t.IncMDD;
            item.IncACC = t.IncACC;
            item.IncMHP = t.IncMHP;
            item.Cursed = t.Cursed;
            item.IncINT = t.IncINT;
            item.IncDEX = t.IncDEX;
            item.IncMAD = t.IncMAD;
            item.IncEVA = t.IncEVA;
            item.IncSTR = t.IncSTR;
            item.IncLUK = t.IncLUK;
            item.IncSpeed = t.IncSpeed;
            item.IncMMP = t.IncMMP;
            item.IncJump = t.IncJump;
            item.Inc = t.Inc;
            item.IncIUC = t.IncIUC;
            item.IncCraft = t.IncCraft;
            item.IncRandVol = t.IncRandVol;
            item.Expinc = t.Expinc;
            item.IncLEV = t.IncLEV;
            item.IncFatigue = t.IncFatigue;
            item.IncMaxHP = t.IncMaxHP;
            item.IncMaxMP = t.IncMaxMP;
            item.IncReqLevel = t.IncReqLevel;
            item.RecoveryHP = t.RecoveryHP;
            item.RecoveryMP = t.RecoveryMP;
            item.ConsumeHP = t.ConsumeHP;
            item.ConsumeMP = t.ConsumeMP;
            item.ReqLevel = t.ReqLevel;
            item.ReqCUC = t.ReqCUC;
            item.ReqRUC = t.ReqRUC;
            item.PadRate = t.PadRate;
            item.MadRate = t.MadRate;
            item.PddRate = t.PddRate;
            item.MddRate = t.MddRate;
            item.AccRate = t.AccRate;
            item.EvaRate = t.EvaRate;
            item.SpeedRate = t.SpeedRate;
            item.MhpRRate = t.MhpRRate;
            item.MmpRRate = t.MmpRRate;
            item.HpR = t.HpR;
            item.MpR = t.MpR;
            item.MmpR = t.MmpR;
            item.MhpR = t.MhpR;
            item.Poison = t.Poison;
            item.Darkness = t.Darkness;
            item.Weakness = t.Weakness;
            item.Seal = t.Seal;
            item.Curse = t.Curse;
        }

        internal static void InitializeItem(out Item item, int itemId) {
            item = new Item(itemId);
            string itemCategory = ItemConstants.GetItemCategory(itemId);
            if (itemCategory == "" || itemCategory == "Special" || itemCategory == "ItemOption") return;

            string subItemCategory = (itemId / 10000).ToString().PadLeft(4, '0');
            string pathToItemImage = $"{itemCategory}/{subItemCategory}.img/{itemId.ToString().PadLeft(8, '0')}";

            List<WzImageProperty> itemProperties = WzProvider.GetWzProperties(WzProvider.Load("Item"), pathToItemImage);
            if (itemProperties == null) return;

            foreach (WzImageProperty node in itemProperties) {
                switch (node.Name) {
                    case "info":
                        foreach (WzImageProperty infoNode in node.WzProperties) {
                            switch (infoNode.Name) {
                                case "slotMax":
                                case "slotMat": // // some items have both of these but one is zero, kind of stupid. just keep the largest value
                                    item.SlotMax = Math.Max(item.SlotMax, infoNode.GetInt());
                                    break;
                                case "price":
                                    item.Price = infoNode.GetInt();
                                    break;
                                case "unitPrice":
                                    item.UnitPrice = infoNode.GetInt();
                                    break;
                                case "pickUpBlock":
                                    item.PickUpBlock = infoNode.GetInt();
                                    break;
                                case "tradBlock":
                                case "tradeBlock": { // some items have both of these but one is zero, kind of stupid. this should be a bool so if one is 1, then keep the 1
                                    int value = item.TradeBlock;
                                    item.TradeBlock = value == 0 ? infoNode.GetInt() : value;
                                    break;
                                }
                                case "maxLevel":
                                    item.MaxLevel = infoNode.GetInt();
                                    break;
                                case "maxDays":
                                    item.MaxDays = infoNode.GetInt();
                                    break;
                                case "questId":
                                    item.QuestId = infoNode.GetInt();
                                    break;
                                case "noCancelMouse":
                                    item.NoCancelMouse = infoNode.GetInt();
                                    break;
                                case "expireOnLogout":
                                    item.ExpireOnLogout = infoNode.GetInt();
                                    break;
                                case "accountSharable":
                                    item.AccountSharable = infoNode.GetInt();
                                    break;
                                case "quest":
                                    item.Quest = infoNode.GetInt();
                                    break;
                                case "only":
                                    item.Only = infoNode.GetInt();
                                    break;
                                case "enchantCategory":
                                    item.EnchantCategory = infoNode.GetInt();
                                    break;
                                case "success":
                                    item.Success = infoNode.GetInt();
                                    break;
                                case "notSale":
                                    item.NotSale = infoNode.GetInt();
                                    break;
                                case "timeLimited":
                                    item.TimeLimited = infoNode.GetInt();
                                    break;
                                case "masterLevel":
                                    item.MasterLevel = infoNode.GetInt();
                                    break;
                                case "incPERIOD":
                                    item.IncPERIOD = (short) infoNode.GetInt();
                                    break;
                                case "incPAD":
                                    item.IncPAD = (short) infoNode.GetInt();
                                    break;
                                case "incMDD":
                                    item.IncMDD = (short) infoNode.GetInt();
                                    break;
                                case "incACC":
                                    item.IncACC = (short) infoNode.GetInt();
                                    break;
                                case "incMHP":
                                    item.IncMHP = (short) infoNode.GetInt();
                                    break;
                                case "cursed":
                                    item.Cursed = (short) infoNode.GetInt();
                                    break;
                                case "incINT":
                                    item.IncINT = (short) infoNode.GetInt();
                                    break;
                                case "incDEX":
                                    item.IncDEX = (short) infoNode.GetInt();
                                    break;
                                case "incMAD":
                                    item.IncMAD = (short) infoNode.GetInt();
                                    break;
                                case "incEVA":
                                    item.IncEVA = (short) infoNode.GetInt();
                                    break;
                                case "incSTR":
                                    item.IncSTR = (short) infoNode.GetInt();
                                    break;
                                case "incLUK":
                                    item.IncLUK = (short) infoNode.GetInt();
                                    break;
                                case "incSpeed":
                                    item.IncSpeed = (short) infoNode.GetInt();
                                    break;
                                case "incMMP":
                                    item.IncMMP = (short) infoNode.GetInt();
                                    break;
                                case "incJump":
                                    item.IncJump = (short) infoNode.GetInt();
                                    break;
                                case "incIUC":
                                    item.IncIUC = (short) infoNode.GetInt();
                                    break;
                                case "incCraft":
                                    item.IncCraft = (short) infoNode.GetInt();
                                    break;
                                case "incRandVol":
                                    item.IncRandVol = (short) infoNode.GetInt();
                                    break;
                                case "incLEV":
                                    item.IncLEV = (short) infoNode.GetInt();
                                    break;
                                case "incMaxHP":
                                    item.IncMaxHP = (short) infoNode.GetInt();
                                    break;
                                case "incMaxMP":
                                    item.IncMaxMP = (short) infoNode.GetInt();
                                    break;
                                case "incReqLevel":
                                    item.IncReqLevel = (short) infoNode.GetInt();
                                    break;
                                case "recoveryHP":
                                    item.RecoveryHP = infoNode.GetInt();
                                    break;
                                case "recoveryMP":
                                    item.RecoveryMP = infoNode.GetInt();
                                    break;
                                case "consumeHP":
                                    item.ConsumeHP = infoNode.GetInt();
                                    break;
                                case "consumeMP":
                                    item.ConsumeMP = (short) infoNode.GetInt();
                                    break;
                                case "reqLevel":
                                    item.ReqLevel = (short) infoNode.GetInt();
                                    break;
                                case "reqCUC":
                                    item.ReqCUC = (short) infoNode.GetInt();
                                    break;
                                case "reqRUC":
                                    item.ReqRUC = (short) infoNode.GetInt();
                                    break;
                            }
                        }

                        break;
                    case "spec":
                        foreach (WzImageProperty specNode in node.WzProperties) {
                            switch (specNode.Name) {
                                case "exp":
                                    item.Exp = specNode.GetInt();
                                    break;
                                case "moveTo":
                                    item.MoveTo = specNode.GetInt();
                                    break;
                                case "consumeOnPickup":
                                    item.ConsumeOnPickup = specNode.GetInt();
                                    break;
                                case "hp":
                                    item.Hp = (short) specNode.GetInt();
                                    break;
                                case "mp":
                                    item.Mp = (short) specNode.GetInt();
                                    break;
                                case "pad":
                                    item.Pad = (short) specNode.GetInt();
                                    break;
                                case "mad":
                                    item.Mad = (short) specNode.GetInt();
                                    break;
                                case "prob":
                                    item.Prob = (short) specNode.GetInt();
                                    break;
                                case "eva":
                                    item.Eva = (short) specNode.GetInt();
                                    break;
                                case "defenseAtt":
                                    item.DefenseAtt = ((WzStringProperty) specNode).Value;
                                    break;
                                case "jump":
                                    item.Jump = (short) specNode.GetInt();
                                    break;
                                case "acc":
                                    item.Acc = (short) specNode.GetInt();
                                    break;
                                case "str":
                                    item.Str = (short) specNode.GetInt();
                                    break;
                                case "luk":
                                    item.Luk = (short) specNode.GetInt();
                                    break;
                                case "int":
                                    item.Int = (short) specNode.GetInt();
                                    break;
                                case "dex":
                                    item.Dex = (short) specNode.GetInt();
                                    break;
                                case "pdd":
                                    item.Pdd = (short) specNode.GetInt();
                                    break;
                                case "mdd":
                                    item.Mdd = (short) specNode.GetInt();
                                    break;
                                case "speed":
                                    item.Speed = (short) specNode.GetInt();
                                    break;
                                case "morph":
                                    item.Morph = (short) specNode.GetInt();
                                    break;
                                case "time":
                                    item.Time = (short) specNode.GetInt();
                                    break;
                                case "inc":
                                    item.Inc = (short) specNode.GetInt();
                                    break;
                                case "expinc":
                                    item.Expinc = (short) specNode.GetInt();
                                    break;
                                case "incFatigue":
                                    item.IncFatigue = (short) specNode.GetInt();
                                    break;
                                case "padRate":
                                    item.PadRate = (short) specNode.GetInt();
                                    break;
                                case "madRate":
                                    item.MadRate = (short) specNode.GetInt();
                                    break;
                                case "pddRate":
                                    item.PddRate = (short) specNode.GetInt();
                                    break;
                                case "mddRate":
                                    item.MddRate = (short) specNode.GetInt();
                                    break;
                                case "accRate":
                                    item.AccRate = (short) specNode.GetInt();
                                    break;
                                case "evaRate":
                                    item.EvaRate = (short) specNode.GetInt();
                                    break;
                                case "speedRate":
                                    item.SpeedRate = (short) specNode.GetInt();
                                    break;
                                case "mhpRRate":
                                    item.MhpRRate = (short) specNode.GetInt();
                                    break;
                                case "mmpRRate":
                                    item.MmpRRate = (short) specNode.GetInt();
                                    break;
                                case "hpR":
                                    item.HpR = (short) specNode.GetInt();
                                    break;
                                case "mpR":
                                    item.MpR = (short) specNode.GetInt();
                                    break;
                                case "mmpR":
                                    item.MmpR = (short) specNode.GetInt();
                                    break;
                                case "mhpR":
                                    item.MhpR = (short) specNode.GetInt();
                                    break;
                                case "poison":
                                    item.Poison = specNode.GetInt();
                                    break;
                                case "darkness":
                                    item.Darkness = specNode.GetInt();
                                    break;
                                case "weakness":
                                    item.Weakness = specNode.GetInt();
                                    break;
                                case "seal":
                                    item.Seal = specNode.GetInt();
                                    break;
                                case "curse":
                                    item.Curse = specNode.GetInt();
                                    break;
                            }
                        }

                        break;
                    case "effect":
                        break;
                    default:
                        Log.Info($"Unhandled Item Property: {node.Name}");
                        break;
                }
            }
        }
    }
}