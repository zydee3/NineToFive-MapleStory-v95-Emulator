using System;
using System.IO;
using log4net;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Scripting;

namespace NineToFive.Event {
    public class ContinueTalkToNpcEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContinueTalkToNpcEvent));

        public ContinueTalkToNpcEvent(Client client) : base(client) { }

        private NpcProperties.ScriptMessageType _messageType;
        private byte _action;

        public override bool OnProcess(Packet p) {
            if (Client.User.ScriptEngine == null) return false;

            byte nMessageType = p.ReadByte();
            if (Enum.IsDefined(typeof(NpcProperties.ScriptMessageType), nMessageType)) {
                _messageType = (NpcProperties.ScriptMessageType) nMessageType;
                _action = p.ReadByte();
                return true;
            }

            return false;
        }

        public override void OnHandle() {
            User user = Client.User;
            NpcScriptMan ctx = user.ScriptEngine.Script.ctx as NpcScriptMan;
            if (ctx == null) return;

            if (_action == 255 || (ctx.Status == 0 && _action == 0)) {
                ctx.Dispose();
                return;
            }

            ctx.Status += _action == 1 ? 1 : -1;

            var npc = user.Field.LifePools[EntityType.Npc][ctx.Npc.Id] as Npc;
            if (npc == null) return;
            if (npc.Field != user.Field) {
                ctx.Dispose();
                return;
            }

            try {
                Scriptable.RunScriptAsync(user.ScriptEngine);
            } catch (Exception e) {
                if (e is AggregateException ae) {
                    ae.Handle(x => {
                        if (x is FileNotFoundException) {
                            user.SendMessage($"Npc Not Found: {npc.TemplateId}");
                        } else {
                            string error = $"Error executing npc: {npc.TemplateId}.js ({e.Message})";
                            Log.Error(error);
                            user.SendMessage(error);
                        }

                        return true;
                    });
                }
            }
        }
    }
}