using System;
using System.IO;
using log4net;
using Microsoft.ClearScript.V8;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Scripting;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class ChatMsgEvent : PacketEvent {
        private static ILog Log = LogManager.GetLogger(typeof(ChatMsgEvent));
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

            if (_msg.StartsWith("!")) {
                using var ctx = new CmdScriptMan(Client, _msg);
                try {
                    using V8ScriptEngine engine = Scriptable.GetEngine($"Commands/{ctx.Name}.js", ctx).Result;
                    Scriptable.RunScriptAsync(engine);
                    return;
                } catch (Exception e) {
                    if (e is AggregateException ae) {
                        ae.Handle(x => {
                            if (x is FileNotFoundException) {
                                user.SendMessage($"Invalid command : '{ctx.Prefix}{ctx.Name}'");
                            } else {
                                Log.Error($"Failed to execute command '{ctx.Prefix}{ctx.Name}'", e);
                                user.SendMessage("The command is not working.");
                            }

                            return true;
                        });
                    }
                }

                return;
            }

            // user.Field.BroadcastPacket(user, GetChatLogMsg($"{user.CharacterStat.Username} : {_msg}"));
            user.Field.BroadcastPacket(user, GetUserMsg(user.CharacterStat.Id, _msg, _shout));
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