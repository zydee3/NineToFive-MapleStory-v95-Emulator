using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Game.Storage.Meta;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class CWvsPackets {
        private static Packet GetMessage(byte type) {
            var w = new Packet();
            w.WriteShort((short) CWvsContext.OnMessage);
            w.WriteByte(type);
            return w;
        }

        public static byte[] GetIncExpMessage(int exp) {
            using var w = GetMessage(3);

            w.WriteBool(true);            // bIsLastHit
            w.WriteInt(exp);              // nIncExp
            var v48 = w.WriteBool(false); // bOnQuest
            w.WriteInt();                 // nSelectedMobBonusExp

            var v6 = w.WriteByte();
            w.WriteByte();
            w.WriteInt(); // nWeddingBonusExp
            if (v6 > 0) w.WriteByte();
            if (v48) {
                var v7 = w.WriteByte();
                if (v7 > 0) w.WriteByte();
            }

            w.WriteByte();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();

            return w.ToArray();
        }

        public static byte[] GetTemporaryStatSet(Skill skill, SkillRecord record) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnTemporaryStatSet);
            skill.EncodeBitmask(w);

            foreach (var pair in skill.CTS) {
                w.WriteShort((short) (int) skill.CTS[pair.Key][record.Level - 1]);
                w.WriteInt(skill.Id);
                w.WriteInt((int) skill.Time[record.Level - 1] * 1000);
            }

            w.WriteByte(); // nDefenseAtt
            w.WriteByte(); // nDefenseState

            if (skill.CTS.ContainsKey(SecondaryStat.SwallowBuff)) w.WriteByte();

            if (skill.CTS.ContainsKey(SecondaryStat.Dice)) {
                for (int i = 0; i < 22; i++) {
                    w.WriteInt();
                }
            }

            if (skill.CTS.ContainsKey(SecondaryStat.BlessingArmor)) w.WriteInt();

            for (int i = 122; i < 129; i++) {
                if (skill.CTS.ContainsKey((SecondaryStat) i)) {
                    if ((SecondaryStat) i == SecondaryStat.GuidedBullet)
                        w.WriteInt();
                    else {
                        w.WriteInt();
                        w.WriteInt();
                    }
                }
            }

            w.WriteShort();

            if (skill.CTS.Any(cts => cts.Key.IsMovementAffectingStat())) {
                w.WriteByte(1);
            }

            w.WriteBytes(new byte[100]);

            return w.ToArray();
        }

        public static byte[] GetTemporaryStatReset(Skill skill) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnTemporaryStatReset);
            skill.EncodeBitmask(w);
            if (skill.CTS.Any(cts => cts.Key.IsMovementAffectingStat())) {
                w.WriteByte();
            }

            return w.ToArray();
        }


        public static byte[] GetForcedStatSet(User user, uint dwcharFlags) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnForcedStatSet);
            w.WriteUInt(dwcharFlags);
            foreach (var type in Enum /**/.GetValues(typeof(ForcedStatType)).Cast<ForcedStatType>()) {
                if (((ForcedStatType) dwcharFlags).HasFlag(type)) {
                    type.EncodeType(user, w);
                }
            }

            return w.ToArray();
        }

        public static byte[] GetForcedStatReset(User user) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnForcedStatReset);
            return w.ToArray();
        }

        public static byte[] GetChangeSkillRecord(Dictionary<int, SkillRecord> skills) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnChangeSkillRecordResult);
            w.WriteBool(true); // get_update_time
            w.WriteShort((short) skills.Count);
            foreach (var record in skills.Values) {
                w.WriteInt(record.Id);
                w.WriteInt(record.Level);
                w.WriteInt(record.MasterLevel);
                w.WriteLong(record.Expiration);
            }

            w.WriteByte();
            return w.ToArray();
        }

        public static byte[] GetStatChanged(User user, uint dwcharFlag) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnStatChanged);
            w.WriteBool(true);
            user.CharacterStat.EncodeChangeStat(user, w, dwcharFlag);
            if (w.WriteBool(false)) {
                w.WriteByte();
            }

            if (w.WriteBool(false)) {
                w.WriteInt(); // nTotalHp
                w.WriteInt(); // nTotalMp
                // CBattleRecordMan::SetBattleRecoveryInfo
            }

            // if ((dwcharFlag & 0x400) == 0x400) ;     // CWvsContext::CheckDarkForce
            // if ((dwcharFlag & 0x1000) == 0x1000) ;   // CWvsContext::CheckDragonFury
            // if ((dwcharFlag & 0x40000) == 0x40000) ; // CWvsContext::CheckQuestCompleteByMeso

            return w.ToArray();
        }

        public static byte[] GetBroadcastMessage(User user, bool whisper, byte type, string msg, ItemSlot item) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnBroadcastMsg);
            w.WriteByte(type);
            switch (type) {
                default: throw new InvalidOperationException("unhandled type : " + type);
                case 0: // blue: [Notice] {msg}
                case 1: // popup: {msg}
                case 5: // pink: {msg}
                case 6: // CUtilDlg::Notice
                case 7: // CUtilDlgEx::CUtilDlgEx
                case 18:
                    w.WriteString(msg);
                    break;
                case 2:  // field megaphone (blue): {username} : {msg}
                case 3:  // megaphone
                case 20: // skull megaphone
                    msg = $"{user.CharacterStat.Username} : {msg}";
                    if (type == 2) goto case 0;
                    w.WriteString(msg);
                    w.WriteByte(user.Client.Channel.Id);
                    w.WriteBool(whisper);
                    break;
                case 4:
                    if (w.WriteBool(msg != null)) {
                        w.WriteString(msg);
                    }

                    break;
                case 8: // item megaphone
                case 9:
                    w.WriteString(msg);
                    w.WriteByte(user.Client.Channel.Id);
                    if (type == 9) break;
                    w.WriteBool(whisper);
                    if (w.WriteBool(item != null)) {
                        item!.Encode(w);
                    }

                    break;
                case 10: // multi-line megaphone
                    w.WriteString(msg);
                    byte count = w.WriteByte();
                    if (count > 2) w.WriteString(msg);
                    if (count > 3) w.WriteString(msg);
                    w.WriteByte(user.Client.Channel.Id);
                    w.WriteBool(whisper);
                    break;
                case 11: // CField::BlowWeather
                    w.WriteString(msg);
                    w.WriteInt(item.TemplateId);
                    break;
                case 12:
                    w.WriteInt(item.TemplateId);
                    w.WriteString(msg);
                    item.Encode(w);
                    break;
                case 13: // {name} duplicated an item from a twin dragons egg congrats
                case 14: // {name} got an item from the twin dragons egg congrats
                    w.WriteString(msg);
                    item.Encode(w);
                    break;
            }

            if (type == 6 || type == 7 || type == 18) {
                // CUtilDlg speaker
                w.WriteInt(2000);
            }

            return w.ToArray();
        }

        public static byte[] GetInventoryOperation(List<InventoryUpdateEntry> updates) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnInventoryOperation);
            w.WriteBool(true); // a3
            w.WriteByte((byte) updates.Count);
            foreach (var entry in updates) {
                ItemSlot item = entry.Item;
                w.WriteByte((byte) entry.Operation);
                w.WriteByte((byte) (item.InventoryType + 1));
                w.WriteShort(entry.PreviousBagIndex);
                switch (entry.Operation) {
                    case InventoryOperation.Add:
                        switch (item) {
                            case ItemSlotBundle bundle:
                                bundle.Encode(w);
                                break;
                            case ItemSlotEquip equip:
                                equip.Encode(w);
                                break;
                            case ItemSlotPet pet:
                                pet.Encode(w);
                                break;
                        }

                        break;
                    case InventoryOperation.Update:
                        w.WriteShort((short) item.Quantity);
                        break;
                    case InventoryOperation.Move:
                        w.WriteShort(item.BagIndex);

                        //LABEL_341:
                        //if ( TSingleton<CUserLocal>::ms_pInstance._m_pStr )
                        //v145 = CInPacket::Decode1(iPacket);
                        if (entry.PreviousBagIndex < 0) {
                            w.WriteByte(1);
                        } else if (item.BagIndex < 0) {
                            w.WriteByte(2);
                        }

                        break;
                    case InventoryOperation.Remove:
                        if (item.BagIndex < 0) {
                            w.WriteByte(2);
                        }

                        break;
                    case InventoryOperation.UpdateStat:
                        w.WriteInt(); // v37
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            return w.ToArray();
        }

        public static byte[] GetChangeSkillRecordResult(List<SkillRecord> records) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnChangeSkillRecordResult);
            w.WriteBool(true);
            w.WriteShort((short) records.Count);
            foreach (SkillRecord record in records) {
                w.WriteInt(record.Id);
                w.WriteInt(record.Level);
                w.WriteInt(record.MasterLevel);
                w.WriteLong(record.Expiration);
            }

            w.WriteByte();
            return w.ToArray();
        }
    }
}