using System;
using System.IO;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Resources;

namespace NineToFive.Wz {
    public static class CharacterWz {
        public static void CopyEquipTemplate(ItemSlotEquip equip) {
            if (equip == null) return;

            int equipId = equip.TemplateId;
            if (!WzCache.EquipTemplates.TryGetValue(equipId, out TemplateEquip t)) {
                InitializeTemplate(out t, equipId);
                WzCache.EquipTemplates.Add(equipId, t);
            }
            
            equip.MaxHP = t.IncMHP;
            equip.MaxMP = t.IncMMP;
            equip.STR = t.IncSTR;
            equip.DEX = t.IncDEX;
            equip.INT = t.IncINT;
            equip.LUK = t.IncLUK;
            equip.PAD = t.IncPAD;
            equip.MAD = t.IncMAD;
            equip.PDD = t.IncPDD;
            equip.MDD = t.IncMDD;
            equip.ACC = t.IncACC;
            equip.EVA = t.IncEVA;
            equip.Speed = t.IncSpeed;
            equip.Jump = t.IncJump;
            equip.RMAF = t.IncRMAF;
            equip.RMAS = t.IncRMAS;
            equip.RMAI = t.IncRMAI;
            equip.RMAL = t.IncRMAL;
            equip.Craft = t.IncCraft;
            equip.MaxHPR = t.IncMHPR;
            equip.MaxMPR = t.IncMMPR;
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
                                templateEquip.ReqLevel = (short) property.GetInt();
                                break;
                            case "reqJob":
                                templateEquip.ReqJob = (short) property.GetInt();
                                break;
                            case "reqSTR":
                                templateEquip.ReqSTR = (short) property.GetInt();
                                break;
                            case "reqDEX":
                                templateEquip.ReqDEX = (short) property.GetInt();
                                break;
                            case "reqINT":
                                templateEquip.ReqINT = (short) property.GetInt();
                                break;
                            case "incLUk":
                            case "reqLUK":
                                templateEquip.ReqLUK = (short) property.GetInt();
                                break;
                            case "reqPOP":
                                templateEquip.ReqPOP = (short) property.GetInt();
                                break;
                            case "incPDD":
                                templateEquip.IncPDD = (short) property.GetInt();
                                break;
                            case "incACC":
                            case "acc":
                                templateEquip.IncACC = (short) property.GetInt();
                                break;
                            case "incDEX":
                                templateEquip.IncDEX = (short) property.GetInt();
                                break;
                            case "incLUK":
                                templateEquip.IncLUK = (short) property.GetInt();
                                break;
                            case "MaxHP":
                            case "maxHP":
                            case "incMHP":
                                templateEquip.IncMHP = (short) property.GetInt();
                                break;
                            case "incMDD":
                                templateEquip.IncMDD = (short) property.GetInt();
                                break;
                            case "incSTR":
                                templateEquip.IncSTR = (short) property.GetInt();
                                break;
                            case "incMMP":
                                templateEquip.IncMMP = (short) property.GetInt();
                                break;
                            case "incSpeed":
                                templateEquip.IncSpeed = (short) property.GetInt();
                                break;
                            case "incJump":
                                templateEquip.IncJump = (short) property.GetInt();
                                break;
                            case "incEVA":
                                templateEquip.IncEVA = (short) property.GetInt();
                                break;
                            case "incINT":
                                templateEquip.IncINT = (short) property.GetInt();
                                break;
                            case "incPAD":
                                templateEquip.IncPAD = (short) property.GetInt();
                                break;
                            case "incMAD":
                                templateEquip.IncMAD = (short) property.GetInt();
                                break;
                            case "incRMAF":
                                templateEquip.IncRMAF = (short) property.GetInt();
                                break;
                            case "incRMAL":
                                templateEquip.IncRMAL = (short) property.GetInt();
                                break;
                            case "incCraft":
                                templateEquip.IncCraft = (short) property.GetInt();
                                break;
                            case "incMHPr":
                                templateEquip.IncMHPR = (short) property.GetInt();
                                break;
                            case "incMMPr":
                                templateEquip.IncMMPR = (short) property.GetInt();
                                break;
                            case "incHP":
                                templateEquip.IncHP = (short) property.GetInt();
                                break;
                            case "incSwim":
                                templateEquip.IncSwim = (short) property.GetInt();
                                break;
                            case "incFatigue":
                                templateEquip.IncFatigue = (short) property.GetInt();
                                break;
                            case "hpRecovery":
                                templateEquip.HPRecovery = (short) property.GetInt();
                                break;
                            case "mpRecovery":
                                templateEquip.MPRecovery = (short) property.GetInt();
                                break;
                            case "attackSpeed":
                                templateEquip.AttackSpeed = (short) property.GetInt();
                                break;
                            case "speed":
                                templateEquip.Speed = (short) property.GetInt();
                                break;
                            case "walk":
                                templateEquip.Walk = (short) property.GetInt();
                                break;
                            case "stand":
                                templateEquip.Stand = (short) property.GetInt();
                                break;
                            case "knockback":
                                templateEquip.KnockBack =(short) property.GetInt();
                                break;
                            case "cash":
                                templateEquip.Cash = property.GetInt();
                                break;
                            case "notSale":
                                templateEquip.NotSale = property.GetInt();
                                break;
                            case "price":
                                templateEquip.Price = property.GetInt();
                                break;
                            case "tradBlock":
                            case "tradeBlock":
                                templateEquip.TradeBlock = property.GetInt();
                                break;
                            case "tuc":
                                templateEquip.TUC = property.GetInt();
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
                                templateEquip.ConsumeHP = property.GetInt();
                                break;
                            case "consumeMP":
                                templateEquip.ConsumeMP = property.GetInt();
                                break;
                            case "longRange":
                                templateEquip.LongRange = ((WzStringProperty) property).Value;
                                break;
                            case "ignorePickup":
                                templateEquip.IgnorePickup = property.GetInt();
                                break;
                            case "islot":
                                templateEquip.ISlot = ((WzStringProperty) property).Value;
                                break;
                            case "vslot":
                                templateEquip.VSlot = ((WzStringProperty) property).Value;
                                break;
                            case "equipTradeBlock":
                                templateEquip.EquipTradeBlock = property.GetInt();
                                break;
                            case "setItemID":
                                templateEquip.SetItemID = property.GetInt();
                                break;
                            case "tradeAvailable":
                                templateEquip.TradeAvailable = property.GetInt();
                                break;
                            case "attack":
                                templateEquip.Attack = (short) property.GetInt();
                                break;
                            case "afterImage":
                                templateEquip.AfterImage = ((WzStringProperty) property).Value;
                                break;
                            case "sfx":
                                templateEquip.Sfx = ((WzStringProperty) property).Value;
                                break;
                            case "only":
                                templateEquip.Only = property.GetInt();
                                break;
                            case "expireOnLogout":
                                templateEquip.ExpireOnLogout = property.GetInt();
                                break;
                            case "slotMax":
                                templateEquip.SlotMax = property.GetInt();
                                break;
                            case "timeLimited":
                                templateEquip.TimeLimited = property.GetInt();
                                break;
                            case "notExtend":
                            case "notExtended":
                                templateEquip.NotExtend = property.GetInt();
                                break;
                            case "durability":
                                templateEquip.Durability = property.GetInt();
                                break;
                            case "hide":
                                templateEquip.Hide = property.GetInt();
                                break;
                            case "quest":
                                templateEquip.Quest = property.GetInt();
                                break;
                            case "weekly":
                                templateEquip.Weekly = property.GetInt();
                                break;
                            case "enchantCategory":
                                templateEquip.EnchantCategory = property.GetInt();
                                break;
                            case "IUCMax":
                                templateEquip.IUCMax = property.GetInt();
                                break;
                            case "fs":
                                templateEquip.Fs = property.GetInt();
                                break;
                            case "medalTag":
                                templateEquip.MedalTag = property.GetInt();
                                break;
                            case "noExpend":
                                templateEquip.NoExpend = property.GetInt();
                                break;
                            case "specialID":
                                templateEquip.SpecialID = property.GetInt();
                                break;
                            case "onlyEquip":
                                templateEquip.OnlyEquip = property.GetInt();
                                break;
                            case "accountSharable":
                                templateEquip.AccountSharable = property.GetInt();
                                break;
                            case "dropBlock":
                                templateEquip.DropBlock = property.GetInt();
                                break;
                            case "pachinko":
                                templateEquip.Pachinko = property.GetInt();
                                break;
                            case "recovery":
                                templateEquip.Recovery = ((WzFloatProperty) property).Value;
                                break;
                            case "chatBalloon":
                                templateEquip.ChatBalloon = property.GetInt();
                                break;
                            case "nameTag":
                                templateEquip.NameTag = property.GetInt();
                                break;
                            case "sharableOnce":
                                templateEquip.SharableOnce = property.GetInt();
                                break;
                            case "tamingMob":
                                templateEquip.TamingMob = property.GetInt();
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