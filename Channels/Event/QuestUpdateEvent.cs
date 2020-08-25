using System;
using System.Drawing;
using System.Numerics;
using NineToFive.Constants;
using NineToFive.Net;
using static NineToFive.Constants.QuestType;

namespace NineToFive.Event {
    public class QuestUpdateEvent : PacketEvent {

        private byte _type;
        private int _targetNpcId;
        private Vector2 _position;
        private short _questId;
        
        public QuestUpdateEvent(Client client) : base(client) { }
        
        public override bool OnProcess(Packet p) {
            return false; // i ain't processing dis till we need it -vincent (08.24.2020 @9:03PM)
            
            _type = p.ReadByte();
            _questId = p.ReadShort();
            _targetNpcId = p.ReadInt();
            _position = new Vector2(p.ReadShort(), p.ReadShort());
            return true;
        }

        public override void OnHandle() {
            switch ((QuestType) _type) {
                case Start:
                case Complete:
                case Resign:
                case StartScriptLinked:
                case CompleteScriptLinked:
                    break;
                default:
                    Console.WriteLine($"Unhandled quest type: {_type}");
                    break;
            }
        }
    }
}