using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Packets;

namespace NineToFive.Scripting {
    public class NpcScriptMan : ScriptManager {
        public NpcScriptMan(Client client, Npc npc) : base(client) {
            Npc = npc;
        }

        public Npc Npc { get; private set; }
        public int Status { get; set; }
        public int Selection { get; set; }
        public bool Proc { get; set; }

        public override void Dispose() {
            User.ScriptEngine?.Dispose();
            User.ScriptEngine = null;
            Npc = null;
            
            base.Dispose();
        }

        public void SendSay(string message) {
            SendSay((byte) NpcProperties.DefaultSpeakerType, message, (byte) NpcProperties.DefaultSpeakerOrientation, false, true);
        }

        public void SendSayPrevNext(string message) {
            SendSay((byte) NpcProperties.DefaultSpeakerType, message, (byte) NpcProperties.DefaultSpeakerOrientation, true, true);
        }

        public void SendSay(byte nSpeakerTypeID, string message, byte bParam, bool bPrev, bool bNext) {
            Client.Session.Write(NpcScriptPackets.GetSay(nSpeakerTypeID, Npc.TemplateId, message, bParam, bPrev, bNext));
            Proc = true;
        }
    }
}