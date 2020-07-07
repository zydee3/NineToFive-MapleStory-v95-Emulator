using System.Linq;
using System.Numerics;
using NineToFive.Net;

namespace NineToFive.Event {
    public class RegisterTeleportEvent : PacketEvent {
        private byte _cy;
        private string _portalName;
        private Vector2 _userLocation;
        private Vector2 _portalLocation;

        public RegisterTeleportEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            _cy = p.ReadByte();
            _portalName = p.ReadString();
            _userLocation = new Vector2(p.ReadShort(), p.ReadShort());
            _portalLocation = new Vector2(p.ReadShort(), p.ReadShort());
            return true;
        }

        public override void OnHandle() {
            var user = Client.User;
            var portal = user.Field.Portals.First(p => p.Name.Equals(_portalName));
            if (portal == null) return;
            var tPortal = user.Field.Portals.First(p => p.Name.Equals(portal.TargetPortalName));
            if (tPortal == null) return;
            Vector2 tPortalLocation = new Vector2(tPortal.X, tPortal.Y);
            if (user.IsDebugging) {
                user.SendMessage($"User-Dist[{Vector2.Distance(_userLocation, user.Location)}]");
                user.SendMessage($"Source[Dist{Vector2.Distance(_userLocation, _portalLocation)}], Target[Dist{Vector2.Distance(_userLocation, tPortalLocation)}]");
            }

            user.Location = tPortalLocation;
        }
    }
}