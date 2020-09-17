using System;
using System.Linq;
using System.Numerics;
using log4net;
using NineToFive.Net;
using NineToFive.Packets;

namespace NineToFive.Event {
    public class TransferFieldEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TransferFieldEvent));
        private int _targetField;
        private string _portal;
        private Vector2 _location;
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
                if (_targetField == 0) {
                    _targetField = Client.User.Field.ReturnMap;
                    return true;
                }

                _location = new Vector2(p.ReadShort(), p.ReadShort());
            }

            p.ReadByte(); // 0
            byte a3 = p.ReadByte();
            byte a4 = p.ReadByte();
            return true;
        }

        public override void OnHandle() {
            var user = Client.User;
            if (user.IsDebugging) {
                user.SendMessage($"From [{user.Field.Id}, {user.CharacterStat.Portal}, {user.Location}], To [{_targetField}, {_portal}, {_location}]");
            }

            if (user.CharacterStat.HP < 1) {
                // user revive
                user.CharacterStat.HP = Math.Max(1, user.CharacterStat.TotalMaxHP / 10);
                user.SetField(user.Field.ReturnMap, null, false);
                return;
            }

            var portal = user.Field.Portals.FirstOrDefault(p => p.Name.Equals(_portal));
            if (portal == null) {
                user.Client.Session.Write(FieldPackets.GetTransferFieldRequestIgnored(4));
                return;
            }

            var dest = Client.Channel.GetField(portal.TargetMap);
            var destPortal = dest.Portals.FirstOrDefault(p => p.Name.Equals(portal.TargetPortalName));
            if (destPortal == null) {
                destPortal = dest.Portals.FirstOrDefault();
                if (destPortal == null) {
                    Log.Warn($"No portal found for field {dest.Id}");
                }
            }

            user.SetField(portal.TargetMap, destPortal, false);
        }
    }
}