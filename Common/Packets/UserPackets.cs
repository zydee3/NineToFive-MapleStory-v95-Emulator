using System;
using System.Collections.Generic;
using System.Linq;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class UserPackets {
        public static byte[] GetKeyMappedInit(Dictionary<int, Tuple<byte, int>> keyMaps = null) {
            using Packet w = new Packet();
            w.WriteShort((short) CFuncKeyMappedMan.OnInit);
            if (w.WriteBool(keyMaps == null)) {
                return w.ToArray();
            }

            for (int i = 0; i < 89; i++) {
                if (!keyMaps!.TryGetValue(i, out var pair)) {
                    w.WriteByte();
                    w.WriteInt();
                } else {
                    w.WriteByte(pair.Item1);
                    w.WriteInt(pair.Item2);
                }
            }

            return w.ToArray();
        }

        public static void EncodeUserRemoteInit(User user, Packet w) {
            w.WriteByte();
            w.WriteString();
            w.WriteString();

            w.WriteShort();
            w.WriteByte();
            w.WriteShort();
            w.WriteByte();

            #region SecondaryStat::DecodeForRemote

            w.WriteLong();
            w.WriteLong();
            w.WriteByte();
            w.WriteByte();

            #endregion

            w.WriteShort();
            user.AvatarLook.Encode(w);
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt(); // chair
            w.WriteShort();
            w.WriteShort();
            w.WriteByte();
            w.WriteShort((short) user.Fh);
            w.WriteByte();

            w.WriteByte(); // CPet::Init_0

            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            if (w.WriteByte() > 0) {
                w.WriteInt();
                w.WriteString();
                w.WriteByte();
                w.WriteByte();
                w.WriteByte();
                w.WriteByte();
                w.WriteByte();
            }

            if (w.WriteByte() > 0) {
                w.WriteString();
            }

            if (w.WriteByte() > 0) {
                w.WriteLong();
                w.WriteLong();
                w.WriteInt();
                // CUserPool::OnCoupleRecordAdd
            }

            if (w.WriteByte() > 0) {
                w.WriteLong();
                w.WriteLong();
                w.WriteInt();
                // CUserPool::OnFriendRecordAdd
            }

            if (w.WriteByte() > 0) {
                w.WriteInt();
                w.WriteInt();
                w.WriteInt();
                // CUserPool::OnMarriageRecordAdd
            }

            byte bCharFlag = w.WriteByte();
            // if ((bCharFlag & 1) == 1) // CUser::LoadDarkForceEffect 
            // if ((bCharFlag & 2) == 2) // CDragon::CreateEffect 
            // if ((bCharFlag & 4) == 4) // CUser::LoadSwallowingEffect 
            if (w.WriteByte() > 0) {
                for (int i = 0; i < w.WriteInt(); i++) {
                    w.WriteInt();
                    // CUserPool::OnNewYearCardRecordAdd
                }
            }

            w.WriteInt();
        }

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
                user.CharacterStat.Encode(w);
                w.WriteByte(); // nPvPExp
                if (w.WriteBool(true)) {
                    w.WriteString();
                }
            }

            if ((dwCharFlag & 2) == 2) {
                w.WriteUInt(user.Money);
            }

            if ((dwCharFlag & 0x80) == 0x80) {
                for (int i = 0; i < 5; i++) {
                    // for-i rather than for-each to explicitly iterate each inventory type sequentially
                    w.WriteByte(user.Inventories[(InventoryType) i].Size);
                }
            }

            if ((dwCharFlag & 0x100000) == 0x100000) {
                w.WriteInt();
                w.WriteInt();
            }

            if ((dwCharFlag & 4) == 4) {
                var eqs = user.Inventories[InventoryType.Equipped].Items;

                foreach (var item in eqs.Where(i => i.BagIndex >= -99)) {
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(w);
                }
                
                w.WriteShort();
                
                foreach (var item in eqs.Where(i => i.BagIndex <= -100)) {
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(w);
                }
                
                w.WriteShort();

                foreach (var item in user.Inventories[InventoryType.Equip].Items) {
                    ItemSlotEquip equip = (ItemSlotEquip) item;
                    w.WriteShort(equip.BagIndex);
                    equip.Encode(w);
                }
                
                w.WriteShort();
                
                //dragon equipment
                w.WriteShort();
                
                //mechanic equipment
                w.WriteShort();
            }

            for (int i = 1; i < 5; i++) {
                int inventoryFlag = 4 << i;
                if ((dwCharFlag & inventoryFlag) == inventoryFlag) {
                    var inventory = user.Inventories[(InventoryType) i];
                    foreach (var item in inventory.Items) {
                        w.WriteByte((byte) item.BagIndex);
                        switch (item) {
                            case ItemSlotBundle bundle:
                                bundle.Encode(w);
                                break;
                            case ItemSlotEquip equip:
                                Console.WriteLine("encoding equip");
                                equip.Encode(w);
                                break;
                            case ItemSlotPet pet:
                                pet.Encode(w);
                                break;
                        }
                    }

                    w.WriteByte();
                }
            }

            if ((dwCharFlag & 0x100) == 0x100) {
                var records = user.Skills.ToArray();
                var count = w.WriteShort((short) records.Length);
                for (int i = 0; i < count; i++) {
                    var record = records[i].Value;
                    w.WriteInt(record.Id);
                    w.WriteInt(record.Level);
                    w.WriteLong(record.Expiration);
                    if (SkillConstants.IsSkillNeedMasterLevel(record.Id)) {
                        w.WriteInt(record.MasterLevel);
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