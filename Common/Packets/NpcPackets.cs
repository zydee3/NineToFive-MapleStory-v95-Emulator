using System.Collections.Generic;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.SendOps;

namespace NineToFive.Packets {
    public static class NpcScriptPackets {
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

            public static byte[] GetNpcImitateData(Npc npc) {
                using Packet w = new Packet();
                w.WriteShort((short) CNpcPool.OnNpcImitateData);
                w.WriteByte(1);
                w.WriteInt(npc.TemplateId);
                w.WriteString(npc.User.CharacterStat.Username);
                npc.User.AvatarLook.Encode(w);
                return w.ToArray();
            }
        }
        
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

        public static byte[] GetAskPet(byte speakerTypeID, int speakerTemplateID, byte param, string message, List<ItemSlotPet> pets) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskPet);
            w.WriteByte(param);
            w.WriteString(message);
            w.WriteByte((byte) pets.Count);
            foreach (ItemSlotPet pet in pets) {
                if (pet != null) {
                    w.WriteLong(pet.TemplateId);
                    w.WriteByte((byte) pet.BagIndex); // i think? CharacterData::FindCashItemSlotPosition
                }
            }

            return w.ToArray();
        }

        public static byte[] GetAskPetAll(byte speakerTypeID, int speakerTemplateID, byte param, string message, bool exceptionExist, List<ItemSlotPet> pets) {
            using Packet w = new Packet();
            w.WriteShort((short) CScriptMan.OnScriptMessage);
            w.WriteByte(speakerTypeID);
            w.WriteInt(speakerTemplateID);
            w.WriteByte((byte) NpcProperties.ScriptMessageType.OnAskPetAll);
            w.WriteByte(param);
            w.WriteString(message);
            w.WriteByte((byte) pets.Count);
            w.WriteBool(exceptionExist);
            foreach (ItemSlotPet pet in pets) {
                if (pet != null) {
                    w.WriteLong(pet.TemplateId);
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
}