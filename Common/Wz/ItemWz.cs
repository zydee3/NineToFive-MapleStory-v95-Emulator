using System;
using System.Collections.Generic;
using log4net;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
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
        public static ItemSlotBundleData GetItemData(int templateId) {
            if (!WzCache.ItemData.TryGetValue(templateId, out ItemSlotBundleData t)) {
                InitializeItem(out t, templateId);
                WzCache.ItemData.Add(templateId, t);
            }

            return t;
        }

        internal static void InitializeItem(out ItemSlotBundleData itemSlotBundleData, int itemId) {
            itemSlotBundleData = new ItemSlotBundleData(itemId);
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
                                    itemSlotBundleData.SlotMax = Math.Max(itemSlotBundleData.SlotMax, infoNode.GetInt());
                                    break;
                                case "price":
                                    itemSlotBundleData.Price = infoNode.GetInt();
                                    break;
                                case "unitPrice":
                                    itemSlotBundleData.UnitPrice = infoNode.GetInt();
                                    break;
                                case "pickUpBlock":
                                    itemSlotBundleData.PickUpBlock = infoNode.GetInt();
                                    break;
                                case "tradBlock":
                                case "tradeBlock": { // some items have both of these but one is zero, kind of stupid. this should be a bool so if one is 1, then keep the 1
                                    int value = itemSlotBundleData.TradeBlock;
                                    itemSlotBundleData.TradeBlock = value == 0 ? infoNode.GetInt() : value;
                                    break;
                                }
                                case "maxLevel":
                                    itemSlotBundleData.MaxLevel = infoNode.GetInt();
                                    break;
                                case "maxDays":
                                    itemSlotBundleData.MaxDays = infoNode.GetInt();
                                    break;
                                case "questId":
                                    itemSlotBundleData.QuestId = infoNode.GetInt();
                                    break;
                                case "noCancelMouse":
                                    itemSlotBundleData.NoCancelMouse = infoNode.GetInt();
                                    break;
                                case "expireOnLogout":
                                    itemSlotBundleData.ExpireOnLogout = infoNode.GetInt();
                                    break;
                                case "accountSharable":
                                    itemSlotBundleData.AccountSharable = infoNode.GetInt();
                                    break;
                                case "quest":
                                    itemSlotBundleData.Quest = infoNode.GetInt();
                                    break;
                                case "only":
                                    itemSlotBundleData.Only = infoNode.GetInt();
                                    break;
                                case "enchantCategory":
                                    itemSlotBundleData.EnchantCategory = infoNode.GetInt();
                                    break;
                                case "success":
                                    itemSlotBundleData.Success = infoNode.GetInt();
                                    break;
                                case "notSale":
                                    itemSlotBundleData.NotSale = infoNode.GetInt();
                                    break;
                                case "timeLimited":
                                    itemSlotBundleData.TimeLimited = infoNode.GetInt();
                                    break;
                                case "masterLevel":
                                    itemSlotBundleData.MasterLevel = infoNode.GetInt();
                                    break;
                                case "incPERIOD":
                                    itemSlotBundleData.IncPERIOD = (short) infoNode.GetInt();
                                    break;
                                case "incPAD":
                                    itemSlotBundleData.IncPAD = (short) infoNode.GetInt();
                                    break;
                                case "incMDD":
                                    itemSlotBundleData.IncMDD = (short) infoNode.GetInt();
                                    break;
                                case "incACC":
                                    itemSlotBundleData.IncACC = (short) infoNode.GetInt();
                                    break;
                                case "incMHP":
                                    itemSlotBundleData.IncMHP = (short) infoNode.GetInt();
                                    break;
                                case "cursed":
                                    itemSlotBundleData.Cursed = (short) infoNode.GetInt();
                                    break;
                                case "incINT":
                                    itemSlotBundleData.IncINT = (short) infoNode.GetInt();
                                    break;
                                case "incDEX":
                                    itemSlotBundleData.IncDEX = (short) infoNode.GetInt();
                                    break;
                                case "incMAD":
                                    itemSlotBundleData.IncMAD = (short) infoNode.GetInt();
                                    break;
                                case "incEVA":
                                    itemSlotBundleData.IncEVA = (short) infoNode.GetInt();
                                    break;
                                case "incSTR":
                                    itemSlotBundleData.IncSTR = (short) infoNode.GetInt();
                                    break;
                                case "incLUK":
                                    itemSlotBundleData.IncLUK = (short) infoNode.GetInt();
                                    break;
                                case "incSpeed":
                                    itemSlotBundleData.IncSpeed = (short) infoNode.GetInt();
                                    break;
                                case "incMMP":
                                    itemSlotBundleData.IncMMP = (short) infoNode.GetInt();
                                    break;
                                case "incJump":
                                    itemSlotBundleData.IncJump = (short) infoNode.GetInt();
                                    break;
                                case "incIUC":
                                    itemSlotBundleData.IncIUC = (short) infoNode.GetInt();
                                    break;
                                case "incCraft":
                                    itemSlotBundleData.IncCraft = (short) infoNode.GetInt();
                                    break;
                                case "incRandVol":
                                    itemSlotBundleData.IncRandVol = (short) infoNode.GetInt();
                                    break;
                                case "incLEV":
                                    itemSlotBundleData.IncLEV = (short) infoNode.GetInt();
                                    break;
                                case "incMaxHP":
                                    itemSlotBundleData.IncMaxHP = (short) infoNode.GetInt();
                                    break;
                                case "incMaxMP":
                                    itemSlotBundleData.IncMaxMP = (short) infoNode.GetInt();
                                    break;
                                case "incReqLevel":
                                    itemSlotBundleData.IncReqLevel = (short) infoNode.GetInt();
                                    break;
                                case "recoveryHP":
                                    itemSlotBundleData.RecoveryHP = infoNode.GetInt();
                                    break;
                                case "recoveryMP":
                                    itemSlotBundleData.RecoveryMP = infoNode.GetInt();
                                    break;
                                case "consumeHP":
                                    itemSlotBundleData.ConsumeHP = infoNode.GetInt();
                                    break;
                                case "consumeMP":
                                    itemSlotBundleData.ConsumeMP = (short) infoNode.GetInt();
                                    break;
                                case "reqLevel":
                                    itemSlotBundleData.ReqLevel = (short) infoNode.GetInt();
                                    break;
                                case "reqCUC":
                                    itemSlotBundleData.ReqCUC = (short) infoNode.GetInt();
                                    break;
                                case "reqRUC":
                                    itemSlotBundleData.ReqRUC = (short) infoNode.GetInt();
                                    break;
                            }
                        }

                        break;
                    case "spec":
                        foreach (WzImageProperty specNode in node.WzProperties) {
                            switch (specNode.Name) {
                                case "exp":
                                    itemSlotBundleData.Exp = specNode.GetInt();
                                    break;
                                case "moveTo":
                                    itemSlotBundleData.MoveTo = specNode.GetInt();
                                    break;
                                case "consumeOnPickup":
                                    itemSlotBundleData.ConsumeOnPickup = specNode.GetInt();
                                    break;
                                case "hp":
                                    itemSlotBundleData.Hp = (short) specNode.GetInt();
                                    break;
                                case "mp":
                                    itemSlotBundleData.Mp = (short) specNode.GetInt();
                                    break;
                                case "pad":
                                    itemSlotBundleData.Pad = (short) specNode.GetInt();
                                    break;
                                case "mad":
                                    itemSlotBundleData.Mad = (short) specNode.GetInt();
                                    break;
                                case "prob":
                                    itemSlotBundleData.Prob = (short) specNode.GetInt();
                                    break;
                                case "eva":
                                    itemSlotBundleData.Eva = (short) specNode.GetInt();
                                    break;
                                case "defenseAtt":
                                    itemSlotBundleData.DefenseAtt = ((WzStringProperty) specNode).Value;
                                    break;
                                case "jump":
                                    itemSlotBundleData.Jump = (short) specNode.GetInt();
                                    break;
                                case "acc":
                                    itemSlotBundleData.Acc = (short) specNode.GetInt();
                                    break;
                                case "str":
                                    itemSlotBundleData.Str = (short) specNode.GetInt();
                                    break;
                                case "luk":
                                    itemSlotBundleData.Luk = (short) specNode.GetInt();
                                    break;
                                case "int":
                                    itemSlotBundleData.Int = (short) specNode.GetInt();
                                    break;
                                case "dex":
                                    itemSlotBundleData.Dex = (short) specNode.GetInt();
                                    break;
                                case "pdd":
                                    itemSlotBundleData.Pdd = (short) specNode.GetInt();
                                    break;
                                case "mdd":
                                    itemSlotBundleData.Mdd = (short) specNode.GetInt();
                                    break;
                                case "speed":
                                    itemSlotBundleData.Speed = (short) specNode.GetInt();
                                    break;
                                case "morph":
                                    itemSlotBundleData.Morph = (short) specNode.GetInt();
                                    break;
                                case "time":
                                    itemSlotBundleData.Time = (short) specNode.GetInt();
                                    break;
                                case "inc":
                                    itemSlotBundleData.Inc = (short) specNode.GetInt();
                                    break;
                                case "expinc":
                                    itemSlotBundleData.Expinc = (short) specNode.GetInt();
                                    break;
                                case "incFatigue":
                                    itemSlotBundleData.IncFatigue = (short) specNode.GetInt();
                                    break;
                                case "padRate":
                                    itemSlotBundleData.PadRate = (short) specNode.GetInt();
                                    break;
                                case "madRate":
                                    itemSlotBundleData.MadRate = (short) specNode.GetInt();
                                    break;
                                case "pddRate":
                                    itemSlotBundleData.PddRate = (short) specNode.GetInt();
                                    break;
                                case "mddRate":
                                    itemSlotBundleData.MddRate = (short) specNode.GetInt();
                                    break;
                                case "accRate":
                                    itemSlotBundleData.AccRate = (short) specNode.GetInt();
                                    break;
                                case "evaRate":
                                    itemSlotBundleData.EvaRate = (short) specNode.GetInt();
                                    break;
                                case "speedRate":
                                    itemSlotBundleData.SpeedRate = (short) specNode.GetInt();
                                    break;
                                case "mhpRRate":
                                    itemSlotBundleData.MhpRRate = (short) specNode.GetInt();
                                    break;
                                case "mmpRRate":
                                    itemSlotBundleData.MmpRRate = (short) specNode.GetInt();
                                    break;
                                case "hpR":
                                    itemSlotBundleData.HpR = (short) specNode.GetInt();
                                    break;
                                case "mpR":
                                    itemSlotBundleData.MpR = (short) specNode.GetInt();
                                    break;
                                case "mmpR":
                                    itemSlotBundleData.MmpR = (short) specNode.GetInt();
                                    break;
                                case "mhpR":
                                    itemSlotBundleData.MhpR = (short) specNode.GetInt();
                                    break;
                                case "poison":
                                    itemSlotBundleData.Poison = specNode.GetInt();
                                    break;
                                case "darkness":
                                    itemSlotBundleData.Darkness = specNode.GetInt();
                                    break;
                                case "weakness":
                                    itemSlotBundleData.Weakness = specNode.GetInt();
                                    break;
                                case "seal":
                                    itemSlotBundleData.Seal = specNode.GetInt();
                                    break;
                                case "curse":
                                    itemSlotBundleData.Curse = specNode.GetInt();
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