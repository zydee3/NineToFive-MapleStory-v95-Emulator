using System;
using NineToFive.Event;
using NineToFive.IO;

namespace NineToFive.Channels.Event {
    public class CharEnterGameEvent : PacketEvent {
        private uint _playerId;
        
        public CharEnterGameEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
        }

        public override bool OnProcess(Packet p) {
            _playerId = p.ReadUInt();
            // obtain ip attached to this user's account
            // compare current ip with Client.LastKnownIp
            // check if user owns _playerId
            return true;
        }

        public override void OnHandle() {
        }
    }
}