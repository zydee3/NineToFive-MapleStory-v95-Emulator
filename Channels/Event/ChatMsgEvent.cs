using System;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class ChatMsgEvent : PacketEvent {
        private string _msg;
        private bool _shout;

        public ChatMsgEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _msg = p.ReadString();
            _shout = p.ReadBool();
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            if (_msg.StartsWith("!pb")) {
                string[] sp = _msg.Split(" ");
                if (sp.Length < 3) {
                    Console.WriteLine("[command] /pb : not enough information");
                    return;
                }

                using Packet w = new Packet();
                for (int i = 1; i < sp.Length; i++) {
                    sbyte sb = sbyte.Parse(sp[i]);
                    w.WriteSByte(sb);
                }

                Console.WriteLine(w.ToArrayString(true));

                Client.Session.Write(w.ToArray());
                return;
            }

            user.Field.BroadcastPacket(user, GetChatLogMsg($"{user.CharacterStat.Username} : {_msg}"));
            // user.Field.BroadcastPacket(user, GetUserMsg(user.CharacterStat.Id, _msg, _shout));
        }

        /// <summary>
        /// displays the message in the in-game chat buffer
        /// </summary>
        private static byte[] GetChatLogMsg(string msg) {
            using Packet w = new Packet();
            w.WriteShort((short) CUserLocal.OnChatMsg);
            w.WriteShort(); // msg type
            w.WriteString(msg);
            return w.ToArray();
        }

        /// <summary>
        /// creates a chat message bubble on the specified character
        /// </summary>
        private static byte[] GetUserMsg(uint characterId, string msg, bool shout) {
            using Packet w = new Packet();
            w.WriteShort((short) CUser.OnChat_Send);
            w.WriteUInt(characterId);
            w.WriteByte(); // unknown
            w.WriteString(msg);
            w.WriteBool(shout);
            return w.ToArray();
        }
    }
}