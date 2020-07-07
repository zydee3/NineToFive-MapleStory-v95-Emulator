using System.Linq;
using System.Numerics;
using log4net;
using NineToFive.Net;

namespace NineToFive.Event {
    public class TransferFieldEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TransferFieldEvent));
        private int _targetField;
        private string _portal;
        private Vector2 _location = Vector2.Zero;
        public TransferFieldEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            if (p.Size - p.Position == 0) {
                // exit cash shop or trading center
                return true;
            }

            p.ReadByte(); // 0
            _targetField = p.ReadInt();
            _portal = p.ReadString();
            if (_portal.Length == 0) {
                _location = new Vector2(p.ReadShort(), p.ReadShort());
            }

            p.ReadByte(); // 0
            byte a3 =p.ReadByte();
            byte a4 = p.ReadByte();

            Log.Info($"{_targetField}, '{_portal}', {a3}, {a4}");
            return true; 
        }

        public override void OnHandle() {
            var user = Client.User;
            var portal = user.Field.Portals.First(p => p.Name.Equals(_portal));
            if (portal == null) {
                Log.Warn($"Failed to find portal '{_portal}' in field {user.Field.Id}");
                return;
            }

            if (_location != Vector2.Zero) user.Location = _location;
            var field = Client.Channel.GetField(portal.TargetMap);
            portal = field.Portals.First(p => p.Name.Equals(portal.Name));
            user.SetField(field.Id, portal);
        }
    }
}