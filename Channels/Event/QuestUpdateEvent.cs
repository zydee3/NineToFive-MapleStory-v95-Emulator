using System;
using System.Numerics;
using NineToFive.Constants;
using NineToFive.Net;

namespace NineToFive.Event {
    public class QuestUpdateEvent : PacketEvent {
        private Vector2 _position;
        private short _questId;
        private int _targetNpcId;
        private byte _type;

        public QuestUpdateEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _type = p.ReadByte();
            _questId = p.ReadShort();
            _targetNpcId = p.ReadInt();
            _position = new Vector2(p.ReadShort(), p.ReadShort());
            return false;
        }

        public override void OnHandle() {
            switch ((QuestType) _type) {
                case QuestType.Start:
                case QuestType.Complete:
                case QuestType.Resign:
                case QuestType.StartScriptLinked:
                case QuestType.CompleteScriptLinked:
                    break;
                default:
                    Console.WriteLine($"Unhandled quest type: {_type}");
                    break;
            }
        }
    }
}