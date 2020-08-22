using System;
using System.Collections.Generic;
using System.Linq;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class DropPool {
        /// <summary>
        /// <code>0    drops from Drop.Origin, disappears when landing</code>
        /// <code>1    drops from Drop.Origin, lands at Drop.Location</code>
        /// <code>2    no animation, instantly appears at Drop.Location</code>
        /// <code>3    drops from Drop.Origin, disappears when falling</code>
        /// <code>4    static image, slowly lands at Drop.Location</code>
        /// </summary>
        /// <param name="drop">drop object</param>
        /// <param name="type">animation type for the drop</param>
        /// <returns></returns>
        public static byte[] GetDropEnterField(Drop drop, byte type) {
            using Packet w = new Packet();
            w.WriteShort((short) CDropPool.OnDropEnterField);
            w.WriteByte(type);
            w.WriteUInt(drop.Id);

            byte a = w.WriteByte();
            w.WriteInt(drop.TemplateId);
            w.WriteInt();
            w.WriteByte((byte) drop.Fh);
            w.WriteShort((short) drop.Location.X); // destination
            w.WriteShort((short) drop.Location.Y);
            w.WriteInt();

            if (type == 0 || type == 1 || type == 3 || type == 4) {
                w.WriteShort((short) drop.Origin.X);
                w.WriteShort((short) drop.Origin.Y);
                w.WriteShort();
            }

            if (a == 0) {
                w.WriteLong();
            }

            w.WriteByte();
            w.WriteBool(false);
            return w.ToArray();
        }

        public static byte[] GetDropLeaveField(Drop drop, byte type) {
            using Packet w = new Packet();
            w.WriteShort((short) CDropPool.OnDropLeaveField);
            w.WriteByte(type);
            w.WriteUInt(drop.Id);

            if (type == 2 || type == 3 || type == 5) w.WriteInt();
            else if (type == 4) w.WriteShort();
            if (type == 5) w.WriteInt();

            return w.ToArray();
        }
    }

    public static class ReactorPool {
        public static byte[] GetReactorChangeState(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorChangeState);
            w.WriteUInt(reactor.Id);
            w.WriteByte(); // CReactorPool *pThis[4]
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            w.WriteByte(); // CReactorPool *pThis[9]
            w.WriteByte(); // CReactorPool *pThis[11]
            return w.ToArray();
        }

        public static byte[] GetReactorMove(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorMove);
            w.WriteUInt(reactor.Id);
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            return w.ToArray();
        }

        public static byte[] GetReactorEnterField(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorEnterField);
            w.WriteUInt(reactor.Id);
            w.WriteInt(reactor.TemplateId);
            w.WriteByte(); // CReactorPool *pThis[4] , // CReactorPool *pThis[3]
            w.WriteShort((short) reactor.Location.X);
            w.WriteShort((short) reactor.Location.Y);
            w.WriteByte(); // CReactorPool *pThis[16] 
            w.WriteString();
            return w.ToArray();
        }

        public static byte[] GetReactorLeaveField(Reactor reactor) {
            using Packet w = new Packet();
            w.WriteShort((short) CReactorPool.OnReactorLeaveField);
            w.WriteUInt(reactor.Id);
            return w.ToArray();
        }
    }

    public static class NpcPool {
        private static void InitNpc(Npc npc, Packet w) {
            w.WriteShort((short) npc.Location.X);
            w.WriteShort((short) npc.Location.Y);
            w.WriteBool(true); // bMove
            w.WriteShort((short) npc.Fh);
            w.WriteShort((short) npc.HorizontalRange.Low);
            w.WriteShort((short) npc.HorizontalRange.High);
            w.WriteBool(true); // bEnabled
        }

        public static byte[] GetNpcEnterField(Npc npc) {
            using Packet w = new Packet();
            w.WriteShort((short) CNpcPool.OnNpcEnterField);
            w.WriteUInt(npc.Id);
            w.WriteInt(npc.TemplateId);
            InitNpc(npc, w);
            return w.ToArray();
        }

        public static byte[] GetNpcLeaveField(Npc npc) {
            using Packet w = new Packet();
            w.WriteShort((short) CNpcPool.OnNpcLeaveField);
            w.WriteUInt(npc.Id);
            return w.ToArray();
        }

        public static byte[] GetNpcChangeController(Npc npc) {
            using Packet w = new Packet();
            w.WriteShort((short) CNpcPool.OnNpcChangeController);
            w.WriteBool(true); // bLocalNpc
            w.WriteUInt(npc.Id);
            return w.ToArray();
        }

        public static byte[] GetUpdateLimitedDisableInfo(Npc npc) {
            using Packet w = new Packet();
            w.WriteShort((short) CNpcPool.OnUpdateLimitedDisableInfo);
            for (byte i = 0; i < w.WriteByte(); i++) {
                w.WriteInt();
            }

            return w.ToArray();
        }

        public static byte[] GetNpcImitateData(Npc npc, User user) {
            using Packet w = new Packet();
            w.WriteShort((short) CNpcPool.OnNpcImitateData);
            w.WriteByte(1);
            w.WriteInt(npc.TemplateId);
            w.WriteString(user.CharacterStat.Username);
            npc.AvatarLook.Encode(null, w);
            return w.ToArray();
        }
    }

    public static class MobPool {
        private static void InitMob(Mob mob, Packet w) {
            w.WriteShort((short) mob.Location.X);
            w.WriteShort((short) mob.Location.Y);
            w.WriteByte(mob.MoveAction);
            w.WriteShort((short) mob.Fh); // cur fh
            w.WriteShort((short) mob.Fh); // home fh
            w.WriteByte((byte) (mob.SummonType == 1 ? 255 : mob.SummonType));
            if (mob.SummonType == -3 || mob.SummonType >= 0) {
                w.WriteInt();
            }

            w.WriteByte(); // carnival team
            w.WriteInt((mob.HP / mob.MaxHP) * 100);
            w.WriteInt(); // nEffectItemID
        }

        private static void SetMobLocal(Mob mob, Packet w, bool init) {
            w.WriteInt(mob.TemplateId);
            if (init) {
                SetMobTemporaryStat(mob, w);
            } else {
                SetMobTemporaryStat(mob, w);
                InitMob(mob, w);
            }
        }

        private static void SetMobTemporaryStat(Mob mob, Packet w) {
            // CMob::SetTemporaryStat flags
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            // MobStat::DecodeTemporary
        }

        public static byte[] GetMobEnterField(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobEnterField);
            w.WriteUInt(mob.Id);
            w.WriteByte(5); // nCalcDamageIndex
            w.WriteInt(mob.TemplateId);

            SetMobTemporaryStat(mob, w);
            InitMob(mob, w);

            return w.ToArray();
        }

        public static byte[] GetMobLeaveField(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobLeaveField);
            w.WriteUInt(mob.Id);
            byte b = w.WriteByte(); // dead type
            if (b == 4) w.WriteInt();
            return w.ToArray();
        }

        public static byte[] GetMobChangeController(Mob mob) {
            using Packet w = new Packet();
            w.WriteShort((short) CMobPool.OnMobChangeController);

            // 1+ for CVecCtrlMob::SetMoveRandManSeed
            // 2 for CMob::ChaseTarget
            var controllerLevel = w.WriteByte((byte) (mob.ChaseTarget ? 2 : 1));
            w.WriteUInt(mob.Id);
            if (controllerLevel > 0) {
                w.WriteByte(5); // nCalcDamageIndex
                w.WriteInt(mob.TemplateId);
            }

            SetMobTemporaryStat(mob, w);
            // CMob::InitMob is only necessary if the controller is being set
            // and the client hasn't registered the mob (CMobPool::GetMob returns false) 
            InitMob(mob, w);

            return w.ToArray();
        }

        public static byte[] GetShowHpIndicator(int mobId, byte health) {
            using Packet w = new Packet();
            w.WriteShort((short) CMob.OnHPIndicator);
            w.WriteInt(mobId);
            w.WriteByte(health);
            return w.ToArray();
        }
    }

    public static class CWvsPackets {
        public static byte[] GetTemporaryStatSet(Skill skill, SkillRecord record) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnTemporaryStatSet);

            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt((int) skill.BitMask);

            foreach (var ts in Enum.GetValues(typeof(TemporaryStat)).Cast<TemporaryStat>()) {
                if ((skill.BitMask & ts) != ts || ts == TemporaryStat.None) continue;
                w.WriteShort((short) ts.GetFromSkill(skill, record));
                w.WriteInt(skill.Id);
                w.WriteInt(skill.Time[record.Level - 1] * 1000);
            }

            w.WriteByte();
            w.WriteByte();

            w.WriteShort(1);
            // SecondaryStat::IsMovementAffectingStat
            w.WriteByte(1);
            return w.ToArray();
        }

        public static byte[] GetTemporaryStatReset(Skill skill) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnTemporaryStatSet);

            w.WriteInt();
            w.WriteInt();
            w.WriteInt();
            w.WriteInt((int) skill.BitMask);

            // SecondaryStat::IsMovementAffectingStat
            w.WriteByte();
            return w.ToArray();
        }

        public static byte[] GetForcedStatSet(User user, int dwcharFlags) {
            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnForcedStatSet);
            w.WriteInt(dwcharFlags);
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

            if ((dwcharFlag & 0x400) == 0x400) ;     // CWvsContext::CheckDarkForce
            if ((dwcharFlag & 0x1000) == 0x1000) ;   // CWvsContext::CheckDragonFury
            if ((dwcharFlag & 0x40000) == 0x40000) ; // CWvsContext::CheckQuestCompleteByMeso

            return w.ToArray();
        }

        public static byte[] GetBroadcastMessage(User user, bool whisper, byte type, string msg, Item item) {
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
        public static byte[] GetKeyMappedInit(Dictionary<int, Tuple<byte, int>> keyMaps = null) {
            using Packet w = new Packet();
            w.WriteShort((short) CFuncKeyMappedMan.OnInit);
            bool useDefault = keyMaps == null;
            if (w.WriteBool(useDefault)) {
                return w.ToArray();
            }

            int count = 89;
            foreach (var pair in keyMaps!) {
                var keyMap = pair.Value;
                w.WriteByte(keyMap.Item1);
                w.WriteInt(keyMap.Item2);
                count--;
            }

            for (int i = 0; i < count; i++) {
                w.WriteByte();
                w.WriteInt();
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
            user.AvatarLook.Encode(user, w);
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
                user.CharacterStat.Encode(user, w);
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
                    item.Encode(item, w);
                }

                w.WriteShort();
                foreach (var item in eqs.Where(i => i.BagIndex <= -100)) {
                    w.WriteShort(Math.Abs(item.BagIndex));
                    item.Encode(item, w);
                }

                w.WriteShort();
                w.WriteShort();
                w.WriteShort();
                w.WriteShort();
            }

            for (int i = 1; i < 5; i++) {
                int inventoryFlag = 4 << (i - 1);
                if ((dwCharFlag & inventoryFlag) == inventoryFlag) {
                    var inventory = user.Inventories[(InventoryType) i];
                    foreach (var item in inventory.Items) {
                        w.WriteByte((byte) item.BagIndex);
                        item.Encode(item, w);
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

    public static class NpcScriptPackets {
        /// <summary>
        /// Generates a packet for OnSay (NpcProperties.ScriptMessageType.OnSay = 0)
        /// </summary>
        /// <param name="speakerTypeID">speaker NpcProperties.SpeakerType</param>
        /// <param name="speakerTemplateID">npc id</param>
        /// <param name="message">npc dialog</param>
        /// <param name="param">The direction the npc / player faces</param>
        /// <param name="prev">Includes Prev button</param>
        /// <param name="next">Includes Next button</param>
        /// <returns></returns>
        public static byte[] GetSay(byte speakerTypeID, int speakerTemplateID, string message, byte param, bool prev, bool next) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte();
            w.WriteByte(param);

            if ((param & 4) == 4)
                w.WriteInt(speakerTemplateID);

            w.WriteString(message);
            w.WriteBool(prev);
            w.WriteBool(next);
            return w.ToArray();
        }

        public static byte[] GetSayImage(byte speakerTypeID, int speakerTemplateID, byte param, List<string> list) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnSayImage);
            w.WriteByte(param);
            w.WriteByte((byte) list.Count); // if ( CInPacket::Decode1(iPacket) > 0 )
            foreach (string s in list) {
                w.WriteString(s);
            }

            return w.ToArray();
        }

        public static byte[] GetAskYesNo(byte speakerTypeID, int speakerTemplateID, byte param, string input) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskYesNo);
            w.WriteByte(param);
            w.WriteString(input);
            return w.ToArray();
        }

        public static byte[] GetAskText(byte speakerTypeID, int speakerTemplateID, byte param, string input, string input2, short lenMin, short lenMax) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskText);
            w.WriteByte(param);
            w.WriteString(input);
            w.WriteString(input2);
            w.WriteShort(lenMin);
            w.WriteShort(lenMax);
            return w.ToArray();
        }

        public static byte[] GetAskNumber(byte speakerTypeID, int speakerTemplateID, byte param, string text, int def, int min, int max) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskNumber);
            w.WriteByte(param);
            w.WriteString(text);
            w.WriteInt(def);
            w.WriteInt(min);
            w.WriteInt(max);
            return w.ToArray();
        }

        public static byte[] GetAskMenu(byte speakerTypeID, int speakerTemplateID, byte param, string text) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskMenu);
            w.WriteByte(param);
            w.WriteString(text);
            return w.ToArray();
        }

        public static byte[] GetAskQuiz(byte speakerTypeID, int speakerTemplateID, byte param, byte v4, string quizTitle, string quizText, string quizHint, int minInput, int maxInput, int remain) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskQuiz);
            w.WriteByte(param);
            w.WriteByte(v4);

            if (v4 == 0) {
                w.WriteString(quizTitle);
                w.WriteString(quizText);
                w.WriteString(quizHint);
                w.WriteInt(minInput);
                w.WriteInt(maxInput);
                w.WriteInt(remain);
            }

            return w.ToArray();
        }

        public static byte[] GetAskSpeedQuiz(byte speakerTypeID, int speakerTemplateID, byte param, byte v4, int type, int dwAnswer, int correct, int remain) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskSpeedQuiz);
            w.WriteByte(param);

            if (v4 == 0) {
                // idk what this is
                w.WriteInt(type);
                w.WriteInt(dwAnswer);
                w.WriteInt(correct);
                w.WriteInt(remain);
            }

            return w.ToArray();
        }

        public static byte[] GetAskAvatar(byte speakerTypeID, int speakerTemplateID, byte param, string text, int[] styles) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskAvataar);
            w.WriteByte(param);
            w.WriteString(text);
            w.WriteByte((byte) styles.Length);
            foreach (int i in styles) {
                w.WriteInt(i);
            }

            return w.ToArray();
        }

        public static byte[] GetAskMembershopAvatar(byte speakerTypeID, int speakerTemplateID, byte param, string text, IEnumerable<int> list) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskMembershopAvatar);
            w.WriteByte(param);
            w.WriteString(text);
            foreach (int i in list) {
                // I have no idea what this is, like absolutely no clue, none, nada, zip
                w.WriteInt(i);
            }

            return w.ToArray();
        }

        public static byte[] GetAskPet(byte speakerTypeID, int speakerTemplateID, byte param, string message, List<Item> pets) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskPet);
            w.WriteByte(param);
            w.WriteString(message);
            w.WriteByte((byte) pets.Count);
            foreach (Item pet in pets) {
                if (pet != null) {
                    w.WriteLong(pet.Id);
                    w.WriteByte((byte) pet.BagIndex); // i think? CharacterData::FindCashItemSlotPosition
                }
            }

            return w.ToArray();
        }

        public static byte[] GetAskPetAll(byte speakerTypeID, int speakerTemplateID, byte param, string message, bool exceptionExist, List<Item> pets) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskPetAll);
            w.WriteByte(param);
            w.WriteString(message);
            w.WriteByte((byte) pets.Count);
            w.WriteBool(exceptionExist);
            foreach (Item pet in pets) {
                if (pet != null) {
                    w.WriteLong(pet.Id);
                    w.WriteByte((byte) pet.BagIndex); // i think? CharacterData::FindCashItemSlotPosition
                }
            }

            return w.ToArray();
        }

        public static byte[] GetAskBoxText(byte speakerTypeID, int speakerTemplateID, byte param, string text, string text1, short col, short line) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskBoxText);
            w.WriteByte(param);
            w.WriteString(text);
            w.WriteString(text1);
            w.WriteShort(col);
            w.WriteShort(line);
            return w.ToArray();
        }

        public static byte[] GetAskSlideMenu(byte speakerTypeID, int speakerTemplateID, byte param, int v2) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskSlideMenu);
            w.WriteByte(param);
            w.WriteInt(v2); // region?
            return w.ToArray();
        }
    }

    public static class FieldPackets {
        /// <summary>
        /// <code>1 for     "The portal is closed for now."</code>
        /// <code>2 for     "You cannot go to that place."</code>
        /// <code>3 for     "Unable to approach due the the force of the ground."</code>
        /// <code>4 for     "You cannot teleport to or on this map"</code>
        /// <code>5 for     "Unable to approach due to the force of the ground."</code>
        /// <code>6 for     "This map can only be entered by party members."</code>
        /// <code>7 for     "Only members of an expedition can enter this map."</code>
        /// <code>8 for     "The Cash Shop is currently not available. Stay Tuned."</code>
        /// </summary>
        /// <param name="message">message type to be displayed</param>
        public static byte[] GetTransferFieldRequestIgnored(byte message) {
            using Packet w = new Packet();
            w.WriteShort((short) CField.OnTransferFieldReqIgnored);
            w.WriteByte(message);
            return w.ToArray();
        }
    }
}