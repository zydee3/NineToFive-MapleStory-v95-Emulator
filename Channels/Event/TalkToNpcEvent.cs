using System;
using System.IO;
using log4net;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Net;
using NineToFive.Scripting;

namespace NineToFive.Event {
    public class TalkToNpcEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TalkToNpcEvent));

        private uint _objectId;

        public TalkToNpcEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _objectId = p.ReadUInt();

            p.ReadShort(); // user x location
            p.ReadShort(); // user y location

            return true;
        }

        public override void OnHandle() {
            User user = Client.User;

            user.ScriptEngine?.Dispose();

            var npc = user.Field.LifePools[EntityType.Npc][_objectId] as Npc;
            if (npc == null) return;

            int templateId = npc.TemplateId;

            if (user.IsDebugging) user.SendMessage($"Object Id = {npc.Id}, Npc Id = {templateId}");

            try {
                user.ScriptEngine = Scriptable.GetEngine($"Npc/{templateId}.js", new NpcScriptMan(Client, npc)).Result;
                Scriptable.RunScriptAsync(user.ScriptEngine);
            } catch (Exception e) {
                if (e is AggregateException ae) {
                    ae.Handle(x => {
                        if (x is FileNotFoundException) {
                            user.SendMessage($"Npc Not Found: {templateId}");
                        } else {
                            string error = $"Error executing npc: {templateId}.js ({e.Message})";
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