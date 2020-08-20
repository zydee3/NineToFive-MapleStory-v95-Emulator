using System;
using System.Linq;
using System.Numerics;
using log4net;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;
using NineToFive.Wz;

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
                if (_targetField == 0) {
                    _targetField = Client.User.Field.ReturnMap;
                    return true;
                }
                
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
            
            bool targetPortalExists = _portal.Equals("");
            Portal portal = null;

            if(targetPortalExists){
                try {
                    portal = user.Field.Portals.First(p => p.Name.Equals(_portal));
                } catch (Exception exception) {
                    Log.Warn($"Failed to find portal '{_portal}' in field {user.Field.Id}");
                    return;
                }
            }

            if (_location != Vector2.Zero) user.Location = _location;
            if (_targetField == 999999999 || _targetField == -1) _targetField = portal.TargetMap;
            
            Field field;
            try {
                field = Client.Channel.GetField(_targetField); // throws map not found and cancels the whole event
            } catch (Exception ignore) {
                field = Client.Channel.GetField((user.Field.Id/1000000)*1000000); // 103000100 -> 103000000 or 100000100 -> 100000000 nearest town
            }
            
            portal = (portal == null ? 
                field.Portals[0] : 
                field.Portals.FirstOrDefault(p => p.TargetPortalName.Equals(portal.Name)));

            user.SetField(field.Id, portal);

            if (!targetPortalExists && user.CharacterStat.HP == 0 && field.Town) {
                user.CharacterStat.HP = 50;
                user.CharacterStat.SendUpdate(user, (uint)UserAbility.HP);
            }
        }
    }
}