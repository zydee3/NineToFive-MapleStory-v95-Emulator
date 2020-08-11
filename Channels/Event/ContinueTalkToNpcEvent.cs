using System;
using System.IO;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
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
            Console.WriteLine(p.ToArrayString(true));
            User user = Client.User;
            if (user.ScriptEngine == null || user.NpcScriptInstance == null) {
                return false;
            }
            
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
            NpcScriptMan scriptInteraction = (NpcScriptMan) user.NpcScriptInstance;
            
            if (_action == 255 || (scriptInteraction.Status == 0 && _action == 0)) {
                user.ScriptEngine.Dispose();
                user.ScriptEngine = null;
                user.NpcScriptInstance.Dispose();
                user.NpcScriptInstance = null;
                return;
            }
            
            int objectId = scriptInteraction.ObjectId;
            scriptInteraction.Status += _action == 1 ? 1 : -1;

            if (user.Field.LifePools.TryGetValue(EntityType.Npc, out LifePool<Life> pool)) {
                Npc npc = (Npc) pool.FindFirst(life => life.Id == objectId);
                if (npc == null)
                    return;

                int npcId = scriptInteraction.NpcId;

                try {
                    Scriptable.RunScriptAsync(user.ScriptEngine).Wait();
                } catch (Exception e) {
                    if (e is AggregateException ae) {
                        ae.Handle(x => {
                            if (x is FileNotFoundException) {
                                user.SendMessage($"Npc Not Found: {npcId}");
                            } else {
                                string error = $"Error executing npc: {npcId}.js ({e.Message})";
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
}