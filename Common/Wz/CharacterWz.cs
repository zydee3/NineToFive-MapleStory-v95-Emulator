using System;
using System.IO;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class CharacterWz {
        public static void CopyEquipTemplate(Equip equip) {
            if (equip == null) return;

            int equipId = equip.Id;
            if (!WzCache.EquipTemplates.TryGetValue(equipId, out TemplateEquip t)) {
                InitializeTemplate(out t, equipId);
                WzCache.EquipTemplates.Add(equipId, t);
            }
            
            equip.IncHP = t.IncHP;
            equip.IncMHP = t.IncMHP;
            equip.IncMP = t.IncMP;
            equip.IncMMP = t.IncMMP;
            equip.IncSTR = t.IncSTR;
            equip.IncDEX = t.IncDEX;
            equip.IncINT = t.IncINT;
            equip.IncLUK = t.IncLUK;
            equip.IncPAD = t.IncPAD;
            equip.IncMAD = t.IncMAD;
            equip.IncPDD = t.IncPDD;
            equip.IncMDD = t.IncMDD;
            equip.IncACC = t.IncACC;
            equip.IncEVA = t.IncEVA;
            equip.IncSpeed = t.IncSpeed;
            equip.IncJump = t.IncJump;
            equip.IncRMAF = t.IncRMAF;
            equip.IncRMAS = t.IncRMAS;
            equip.IncRMAI = t.IncRMAI;
            equip.IncRMAL = t.IncRMAL;
            equip.IncCraft = t.IncCraft;
            equip.IncMHPR = t.IncMHPR;
            equip.IncMMPR = t.IncMMPR;
            equip.IncSwim = t.IncSwim;
            equip.IncFatigue = t.IncFatigue;
            equip.AttackSpeed = t.AttackSpeed;
            equip.HPRecovery = t.HPRecovery;
            equip.MPRecovery = t.MPRecovery;
            equip.KnockBack = t.KnockBack;
            equip.Walk = t.Walk;
            equip.Stand = t.Stand;
            equip.Speed = (short) t.Speed;
            equip.ReqLevel = t.ReqLevel;
            equip.ReqJob = t.ReqJob;
            equip.ReqSTR = t.ReqSTR;
            equip.ReqDEX = t.ReqDEX;
            equip.ReqINT = t.ReqINT;
            equip.ReqLUK = t.ReqLUK;
            equip.ReqPOP = t.ReqPOP;
            equip.Price = t.Price;
            equip.NotSale = t.NotSale;
            equip.TradeBlock = t.TradeBlock;
            equip.EquipTradeBlock = t.EquipTradeBlock;
            equip.AccountSharable = t.AccountSharable;
            equip.DropBlock = t.DropBlock;
            equip.SlotMax = t.SlotMax;
            equip.TimeLimited = t.TimeLimited;
            equip.TradeAvailable = t.TradeAvailable;
            equip.ExpireOnLogout = t.ExpireOnLogout;
            equip.NotExtend = t.NotExtend;
            equip.OnlyEquip = t.OnlyEquip;
            equip.Pachinko = t.Pachinko;
            equip.ChatBalloon = t.ChatBalloon;
            equip.NameTag = t.NameTag;
            equip.SharableOnce = t.SharableOnce;
            equip.TamingMob = t.TamingMob;
            equip.TUC = t.TUC;
            equip.Cash = t.Cash;
            equip.IgnorePickup = t.IgnorePickup;
            equip.SetItemID = t.SetItemID;
            equip.Only = t.Only;
            equip.Durability = t.Durability;
            equip.ElemDefault = t.ElemDefault;
            equip.ScanTradeBlock = t.ScanTradeBlock;
            equip.EpicItem = t.EpicItem;
            equip.Hide = t.Hide;
            equip.Quest = t.Quest;
            equip.Weekly = t.Weekly;
            equip.EnchantCategory = t.EnchantCategory;
            equip.IUCMax = t.IUCMax;
            equip.Fs = t.Fs;
            equip.MedalTag = t.MedalTag;
            equip.NoExpend = t.NoExpend;
            equip.SpecialID = t.SpecialID;
            equip.VSlot = t.VSlot;
            equip.ISlot = t.ISlot;
            equip.AfterImage = t.AfterImage;
            equip.Sfx = t.Sfx;
            equip.PickupMeso = t.PickupMeso;
            equip.PickupItem = t.PickupItem;
            equip.PickupOthers = t.PickupOthers;
            equip.SweepForDrop = t.SweepForDrop;
            equip.ConsumeHP = t.ConsumeHP;
            equip.LongRange = t.LongRange;
            equip.ConsumeMP = t.ConsumeMP;
            equip.Recovery = t.Recovery;
            equip.Attack = t.Attack;
        }

        internal static void InitializeTemplate(out TemplateEquip templateEquip, int equipId) {
            string targetDirectory = ItemConstants.GetEquipCategory(equipId);
            if (targetDirectory.Equals("")) throw new InvalidDataException($"Unable to locate path to {equipId} in Character.Wz");

            templateEquip = new TemplateEquip(equipId);

            foreach (WzDirectory directory in WzProvider.Load("Character").WzDirectory.WzDirectories) {
                if (!targetDirectory.Equals(directory.Name)) continue;

                string targetImage = $"{equipId}.img".PadLeft(12, '0');
                foreach (WzImage image in directory.WzImages) {
                    if (!image.Name.Equals(targetImage)) continue;
                    WzImageProperty infoNode = image.GetFromPath("info");
                    if (infoNode == null) continue;

                    foreach (WzImageProperty property in infoNode.WzProperties) {
                        if (property == null) continue;

                        switch (property.Name) {
                            case "reqLevel":
                                templateEquip.ReqLevel = ((WzIntProperty) property).Value;
                                break;
                            case "reqJob":
                                templateEquip.ReqJob = ((WzIntProperty) property).Value;
                                break;
                            case "reqSTR":
                                templateEquip.ReqSTR = ((WzIntProperty) property).Value;
                                break;
                            case "reqDEX":
                                templateEquip.ReqDEX = ((WzIntProperty) property).Value;
                                break;
                            case "reqINT":
                                templateEquip.ReqINT = ((WzIntProperty) property).Value;
                                break;
                            case "incLUk":
                            case "reqLUK":
                                templateEquip.ReqLUK = ((WzIntProperty) property).Value;
                                break;
                            case "reqPOP":
                                templateEquip.ReqPOP = ((WzIntProperty) property).Value;
                                break;
                            case "incPDD":
                                templateEquip.IncPDD = ((WzIntProperty) property).Value;
                                break;
                            case "incACC":
                            case "acc":
                                templateEquip.IncACC = ((WzIntProperty) property).Value;
                                break;
                            case "incDEX":
                                templateEquip.IncDEX = ((WzIntProperty) property).Value;
                                break;
                            case "incLUK":
                                templateEquip.IncLUK = ((WzIntProperty) property).Value;
                                break;
                            case "MaxHP":
                            case "maxHP":
                            case "incMHP":
                                templateEquip.IncMHP = ((WzIntProperty) property).Value;
                                break;
                            case "incMDD":
                                templateEquip.IncMDD = ((WzIntProperty) property).Value;
                                break;
                            case "incSTR":
                                templateEquip.IncSTR = ((WzIntProperty) property).Value;
                                break;
                            case "incMMP":
                                templateEquip.IncMMP = ((WzIntProperty) property).Value;
                                break;
                            case "incSpeed":
                                templateEquip.IncSpeed = ((WzIntProperty) property).Value;
                                break;
                            case "incJump":
                                templateEquip.IncJump = ((WzIntProperty) property).Value;
                                break;
                            case "incEVA":
                                templateEquip.IncEVA = ((WzIntProperty) property).Value;
                                break;
                            case "incINT":
                                templateEquip.IncINT = ((WzIntProperty) property).Value;
                                break;
                            case "incPAD":
                                templateEquip.IncPAD = ((WzIntProperty) property).Value;
                                break;
                            case "incMAD":
                                templateEquip.IncMAD = ((WzIntProperty) property).Value;
                                break;
                            case "incRMAF":
                                templateEquip.IncRMAF = ((WzIntProperty) property).Value;
                                break;
                            case "incRMAL":
                                templateEquip.IncRMAL = ((WzIntProperty) property).Value;
                                break;
                            case "incCraft":
                                templateEquip.IncCraft = ((WzIntProperty) property).Value;
                                break;
                            case "incMHPr":
                                templateEquip.IncMHPR = ((WzIntProperty) property).Value;
                                break;
                            case "incMMPr":
                                templateEquip.IncMMPR = ((WzIntProperty) property).Value;
                                break;
                            case "incHP":
                                templateEquip.IncHP = ((WzIntProperty) property).Value;
                                break;
                            case "incSwim":
                                templateEquip.IncSwim = ((WzIntProperty) property).Value;
                                break;
                            case "incFatigue":
                                templateEquip.IncFatigue = ((WzIntProperty) property).Value;
                                break;
                            case "hpRecovery":
                                templateEquip.HPRecovery = ((WzIntProperty) property).Value;
                                break;
                            case "mpRecovery":
                                templateEquip.MPRecovery = ((WzIntProperty) property).Value;
                                break;
                            case "attackSpeed":
                                templateEquip.AttackSpeed = ((WzIntProperty) property).Value;
                                break;
                            case "speed":
                                templateEquip.Speed = ((WzIntProperty) property).Value;
                                break;
                            case "cash":
                                templateEquip.Cash = ((WzIntProperty) property).Value;
                                break;
                            case "notSale":
                                templateEquip.NotSale = ((WzIntProperty) property).Value;
                                break;
                            case "price":
                                templateEquip.Price = ((WzIntProperty) property).Value;
                                break;
                            case "tradBlock":
                            case "tradeBlock":
                                templateEquip.TradeBlock = ((WzIntProperty) property).Value;
                                break;
                            case "tuc":
                                templateEquip.TUC = ((WzIntProperty) property).Value;
                                break;
                            case "pickupMeso":
                                templateEquip.PickupMeso = ((WzStringProperty) property).Value;
                                break;
                            case "pickupItem":
                                templateEquip.PickupItem = ((WzStringProperty) property).Value;
                                break;
                            case "pickupOthers":
                                templateEquip.PickupOthers = ((WzStringProperty) property).Value;
                                break;
                            case "sweepForDrop":
                                templateEquip.SweepForDrop = ((WzStringProperty) property).Value;
                                break;
                            case "consumeHP":
                                templateEquip.ConsumeHP = ((WzStringProperty) property).Value;
                                break;
                            case "consumeMP":
                                templateEquip.ConsumeMP = ((WzStringProperty) property).Value;
                                break;
                            case "longRange":
                                templateEquip.LongRange = ((WzStringProperty) property).Value;
                                break;
                            case "ignorePickup":
                                templateEquip.IgnorePickup = ((WzIntProperty) property).Value;
                                break;
                            case "islot":
                                templateEquip.ISlot = ((WzStringProperty) property).Value;
                                break;
                            case "vslot":
                                templateEquip.VSlot = ((WzStringProperty) property).Value;
                                break;
                            case "equipTradeBlock":
                                templateEquip.EquipTradeBlock = ((WzIntProperty) property).Value;
                                break;
                            case "setItemID":
                                templateEquip.SetItemID = ((WzIntProperty) property).Value;
                                break;
                            case "tradeAvailable":
                                templateEquip.TradeAvailable = ((WzIntProperty) property).Value;
                                break;
                            case "walk":
                                templateEquip.Walk = ((WzIntProperty) property).Value;
                                break;
                            case "stand":
                                templateEquip.Stand = ((WzIntProperty) property).Value;
                                break;
                            case "attack":
                                templateEquip.Attack = ((WzShortProperty) property).Value;
                                break;
                            case "afterImage":
                                templateEquip.AfterImage = ((WzStringProperty) property).Value;
                                break;
                            case "sfx":
                                templateEquip.Sfx = ((WzStringProperty) property).Value;
                                break;
                            case "only":
                                templateEquip.Only = ((WzIntProperty) property).Value;
                                break;
                            case "expireOnLogout":
                                templateEquip.ExpireOnLogout = ((WzIntProperty) property).Value;
                                break;
                            case "knockback":
                                templateEquip.KnockBack = ((WzIntProperty) property).Value;
                                break;
                            case "slotMax":
                                templateEquip.SlotMax = ((WzIntProperty) property).Value;
                                break;
                            case "timeLimited":
                                templateEquip.TimeLimited = ((WzIntProperty) property).Value;
                                break;
                            case "notExtend":
                            case "notExtended":
                                templateEquip.NotExtend = ((WzIntProperty) property).Value;
                                break;
                            case "durability":
                                templateEquip.Durability = ((WzIntProperty) property).Value;
                                break;
                            case "hide":
                                templateEquip.Hide = ((WzIntProperty) property).Value;
                                break;
                            case "quest":
                                templateEquip.Quest = ((WzIntProperty) property).Value;
                                break;
                            case "weekly":
                                templateEquip.Weekly = ((WzIntProperty) property).Value;
                                break;
                            case "enchantCategory":
                                templateEquip.EnchantCategory = ((WzIntProperty) property).Value;
                                break;
                            case "IUCMax":
                                templateEquip.IUCMax = ((WzIntProperty) property).Value;
                                break;
                            case "fs":
                                templateEquip.Fs = ((WzIntProperty) property).Value;
                                break;
                            case "medalTag":
                                templateEquip.MedalTag = ((WzIntProperty) property).Value;
                                break;
                            case "noExpend":
                                templateEquip.NoExpend = ((WzIntProperty) property).Value;
                                break;
                            case "specialID":
                                templateEquip.SpecialID = ((WzIntProperty) property).Value;
                                break;
                            case "onlyEquip":
                                templateEquip.OnlyEquip = ((WzIntProperty) property).Value;
                                break;
                            case "accountSharable":
                                templateEquip.AccountSharable = ((WzIntProperty) property).Value;
                                break;
                            case "dropBlock":
                                templateEquip.DropBlock = ((WzIntProperty) property).Value;
                                break;
                            case "pachinko":
                                templateEquip.Pachinko = ((WzIntProperty) property).Value;
                                break;
                            case "recovery":
                                templateEquip.Recovery = ((WzFloatProperty) property).Value;
                                break;
                            case "chatBalloon":
                                templateEquip.ChatBalloon = ((WzIntProperty) property).Value;
                                break;
                            case "nameTag":
                                templateEquip.NameTag = ((WzIntProperty) property).Value;
                                break;
                            case "sharableOnce":
                                templateEquip.SharableOnce = ((WzIntProperty) property).Value;
                                break;
                            case "tamingMob":
                                templateEquip.TamingMob = ((WzIntProperty) property).Value;
                                break;

                            // end
                            case "bonusExp":
                            case "replace":
                            case "PotionDiscount":
                            case "keywordEffect":
                            case "addtion":
                            case "effect":
                            case "level":
                            case "addition":
                            case "epic":
                            case "additon":
                            case "transform":
                                //todo MapleLib.WzLib.WzProperties.WzSubProperty
                                break;
                            case "origin":
                            case "icon":
                            case "iconRaw":
                            case "sample":
                                break;
                            case "despair":
                            case "love":
                            case "shine":
                            case "blaze":
                            case "hum":
                            case "bowing":
                            case "hot":
                                break;
                            default:
                                Console.WriteLine($"Unhandled equip property: {property.Name}");
                                break;
                        }
                    }
                }
            }
        }
    }
}