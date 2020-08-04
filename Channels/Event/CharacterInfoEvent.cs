using System;
using System.Linq;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class CharacterInfoEvent : PacketEvent {
        private uint _playerId;

        public CharacterInfoEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Write(CWvsPackets.GetStatChanged(Client.User, 0));
        }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _playerId = p.ReadUInt();
            p.ReadByte();
            return true;
        }

        public override void OnHandle() {
            var user = Client.User.Field.LifePools[EntityType.User].FindFirst(life => ((User) life).CharacterStat.Id == _playerId) as User;
            if (user == null) return;
            Client.Session.Write(GetCharacterInfo(user));
        }

        public static byte[] GetCharacterInfo(User user) {
            var stat = user.CharacterStat;

            using Packet w = new Packet();
            w.WriteShort((short) CWvsContext.OnCharacterInfo);
            w.WriteUInt(stat.Id);
            w.WriteByte(stat.Level);
            w.WriteShort(stat.Job);
            w.WriteShort(stat.Popularity);
            w.WriteBool(false); // married
            w.WriteString();    // guild
            w.WriteString();    // alliance
            w.WriteByte();

            if (w.WriteByte() > 0) {
                #region CUIUserInfo::SetMultiPetInfo

                w.WriteInt();
                w.WriteString();
                w.WriteByte();
                w.WriteShort();
                w.WriteByte();
                w.WriteShort();
                w.WriteInt();
                w.WriteByte();

                #endregion
            }

            if (w.WriteBool(false)) {
                #region CUIUserInfo::SetTamingMobInfo

                w.WriteInt();
                w.WriteInt();
                w.WriteInt();

                #endregion
            }

            #region CUIUserInfo::SetWishItemInfo

            for (int i = 0; i < w.WriteByte(); i++) {
                w.WriteInt();
            }

            #endregion

            #region MedalAchievementInfo::Decode

            w.WriteInt(user.Inventories[InventoryType.Equipped][-49]?.Id ?? 0);
            for (int i = 0; i < w.WriteShort(); i++) {
                w.WriteShort();
            }

            #endregion

            var chairs = user.Inventories[InventoryType.Setup].Items.Where(i => i.Id / 10000 == 301).ToArray();
            w.WriteInt(chairs.Length);
            foreach (var chair in chairs) {
                w.WriteInt(chair.Id);
            }

            return w.ToArray();
        }
    }
}