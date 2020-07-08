﻿using System;
using System.Linq;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class CWvsPackets {
        public static byte[] GetBroadcastMessage(in User user, bool whisper, byte type, string msg, in Item item) {
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
                        item!.Encode(item, w);
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
                    w.WriteInt(item.Id);
                    break;
                case 12:
                    w.WriteInt(item.Id);
                    w.WriteString(msg);
                    item.Encode(item, w);
                    break;
                case 13: // {name} duplicated an item from a twin dragons egg congrats
                case 14: // {name} got an item from the twin dragons egg congrats
                    w.WriteString(msg);
                    item.Encode(item, w);
                    break;
            }

            if (type == 6 || type == 7 || type == 18) {
                // CUtilDlg speaker
                w.WriteInt(2000);
            }
            return w.ToArray();
        }
    }

    public static class UserPackets {
        public static void EncodeCharacterData(User user, Packet w, long dwCharFlag) {
            w.WriteLong(dwCharFlag);
            w.WriteByte();
            byte unk1 = w.WriteByte();
            if (unk1 > 0) {
                w.WriteByte();
                int count = w.WriteInt();
                for (int i = 0; i < count; i++) {
                    w.WriteLong();
                }

                count = w.WriteInt();
                for (int i = 0; i < count; i++) {
                    w.WriteLong();
                }
            }

            if ((dwCharFlag & 1) == 1) {
                user.CharacterStat.Encode(user, w);
                w.WriteByte(); // nPvPExp
                if (w.WriteBool(true)) {
                    w.WriteString();
                }
            }

            if ((dwCharFlag & 2) == 2) {
                w.WriteInt(); // money
            }

            if ((dwCharFlag & 0x80) == 0x80) {
                for (int i = 0; i < 5; i++) {
                    // for-i rather than for-each to explicitly iterate each inventory type sequentially
                    w.WriteByte(user.Inventories[(InventoryType) i].Size);
                }
            }

            if ((dwCharFlag & 0x100000) == 0x100000) {
                w.WriteInt(); // sCharacterName
                w.WriteInt(); // nSkin
            }

            if ((dwCharFlag & 4) == 4) {
                var equipped = user.Inventories[InventoryType.Equipped];
                var equips = user.Inventories[InventoryType.Equip];

                foreach (var item in equipped.Items.Where(i => i.BagIndex > -100)) {
                    // equipped cash
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
                foreach (var item in equipped.Items.Where(i => i.BagIndex < -99)) {
                    // equipped regular
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
                foreach (var item in equips.Items) {
                    // equip
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
                foreach (var item in equipped.Items.Where(i => i.BagIndex > -1100 && i.BagIndex < -1000)) {
                    // equip dragon?
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
                foreach (var item in equipped.Items.Where(i => i.BagIndex < -1100)) {
                    // equip dragon?
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
            }

            for (int i = 1; i < 5; i++) {
                var inventory = user.Inventories[(InventoryType) i];
                foreach (var item in inventory.Items) {
                    w.WriteByte((byte) item.BagIndex);
                    item.Encode(item, w);
                }

                w.WriteByte();
            }

            if ((dwCharFlag & 0x100) == 0x100) {
                for (int i = 0; i < w.WriteShort(); i++) {
                    int jobId = w.WriteInt();
                    w.WriteInt();
                    w.WriteLong();
                    if (SkillConstants.IsSkillNeedMasterLevel(jobId)) {
                        w.WriteInt();
                    }
                }
            }

            if ((dwCharFlag & 0x8000) == 0x8000) {
                // mSkillCooltime
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteInt();
                    w.WriteShort();
                }
            }

            if ((dwCharFlag & 0x200) == 0x200) {
                // CharacterData::SetQuest 
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteShort();
                    w.WriteString();
                }
            }

            if ((dwCharFlag & 0x4000) == 0x4000) {
                // CharacterData::SetQuestComplete
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteShort();
                    w.WriteLong();
                }
            }

            if ((dwCharFlag & 0x400) == 0x400) {
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteInt(); // dwOwnerID
                    w.WriteInt(); // sOwnerID
                    w.WriteInt(); // lRewardGradeQ.Alnod
                    w.WriteInt(); // lRewardGradeQ.Myhead
                    w.WriteInt(); // lRewardGradeQ.Mysize
                }
            }

            if ((dwCharFlag & 0x800) == 0x800) {
                for (int i = 0; i < w.WriteShort(); i++) {
                    EncodeCoupleRecord(w);
                }

                for (int i = 0; i < w.WriteShort(); i++) {
                    EncodeFriendRecord(w);
                }

                for (int i = 0; i < w.WriteShort(); i++) {
                    EncodeMarriageRecord(w);
                }
            }

            if ((dwCharFlag & 0x1000) == 0x1000) {
                for (int i = 0; i < 5; i++) {
                    w.WriteInt(999999999);
                }

                for (int i = 0; i < 10; i++) {
                    w.WriteInt(999999999);
                }
            }

            if ((dwCharFlag & 0x40000) == 0x40000) {
                // GW_NewYearCardRecord::Decode
                // not used in higher versions so it's best to just ignore this
                w.WriteShort();
            }

            if ((dwCharFlag & 0x80000) == 0x80000) {
                // CharacterData::InitQuestExFromRawStr
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteShort();  // nQuestID
                    w.WriteString(); // rawStr
                }
            }

            if ((dwCharFlag & 0x200000) == 0x200000) {
                // GW_WildHunterInfo::Decode
                w.WriteByte();
                for (int i = 0; i < 5; i++) {
                    w.WriteInt();
                }
            }

            if ((dwCharFlag & 0x400000) == 0x400000) {
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteShort();
                    w.WriteLong();
                }
            }

            if ((dwCharFlag & 0x800000) == 0x800000) {
                for (int i = 0; i < w.WriteShort(); i++) {
                    w.WriteShort();
                    w.WriteShort();
                }
            }
        }

        public static void EncodeCoupleRecord(Packet w) {
            // 33 bytes (0x21)
            w.WriteInt();               // dwPairCharacterID
            w.WriteStringFixed("", 13); // sPairCharacterName
            w.WriteLong();              // liSN
            w.WriteLong();              // liPairSN
        }

        public static void EncodeFriendRecord(Packet w) {
            // 37 bytes (0x25)
            w.WriteInt();               // dwPairCharacterID
            w.WriteStringFixed("", 13); // sPairCharacterName
            w.WriteLong();              // liSN
            w.WriteLong();              // liPairSN
            w.WriteInt();               // dwFriendItemID
        }

        public static void EncodeMarriageRecord(Packet w) {
            // 48 bytes (0x30)
            w.WriteInt();               // dwMarriageNo
            w.WriteInt();               // dwGroomID
            w.WriteInt();               // dwBrideID
            w.WriteShort();             // status
            w.WriteInt();               // nGroomItemID
            w.WriteInt();               // nBrideItemID
            w.WriteStringFixed("", 13); // sGroomName
            w.WriteStringFixed("", 13); // sBrideName
        }
    }
}