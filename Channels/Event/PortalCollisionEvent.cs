using System.Numerics;
using NineToFive.Net;

namespace NineToFive.Event {
    public class PortalCollisionEvent : PacketEvent {
        private byte _ptrField;
        private string _name;
        private Vector2 _location;

        public PortalCollisionEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _ptrField = p.ReadByte();
            _name = p.ReadString();
            _location = new Vector2(p.ReadShort(), p.ReadShort());
            return true;
        }

        public override void OnHandle() {
            if (Client.User.IsDebugging) Client.User.SendMessage($"{_ptrField}, {_name}, {_location.ToString()}");
        }
    }
}