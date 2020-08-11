using NineToFive.Constants;
using NineToFive.Packets;

namespace NineToFive.Scripting {
    public class NpcScriptMan : ScriptManager {
        
        public NpcScriptMan(Client client, int objectId, int npcId) : base(client) {
            ObjectId = objectId;
            NpcId = npcId;
            Status = 0;
        }
        
        public int ObjectId { get; set; }
        public int NpcId { get; set; }
        public int Status { get; set; }
        public int Selection { get; set; }

        public void SendSay(string message) {
            SendSay(NpcProperties.DefaultSpeakerType, message, NpcProperties.DefaultSpeakerOrientation, false, true);
        }

        public void SendSayPrevNext(string message) {
            SendSay(NpcProperties.DefaultSpeakerType, message, NpcProperties.DefaultSpeakerOrientation, true, true);
        }

        public void SendSay(byte nSpeakerTypeID, string message, byte bParam, bool bPrev, bool bNext) {
            Client.Session.Write(NpcPackets.GetSay(nSpeakerTypeID, NpcId, message, bParam, bPrev, bNext));
        }
    }
    

}