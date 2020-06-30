using System;
using NineToFive.Game.Storage;
using NineToFive.IO;
using NineToFive.ReceiveOps;

namespace NineToFive.Event {
    public class NewCharEvent : PacketEvent {
        private string _username;
        private int _jobIndex;
        private int[] _avatarLook;
        private byte _gender;
        public NewCharEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Write(GetCreateNewCharFailed(1));
        }

        public override bool OnProcess(Packet p) {
            if (Client.User == null) return false;

            p.Position = 0; // awesome.
            short op = p.ReadShort();
            _username = p.ReadString();
            _jobIndex = p.ReadInt();

            if (!Client.User.CharacterStat.Username.Equals(_username, StringComparison.Ordinal)) {
                Client.Session.Write(GetCreateNewCharFailed(30));
                return false;
            }

            int countAl; // amount of elements in AvatarLook array
            if (op == (int) CLogin.OnNewCharPacket22) {
                p.ReadShort();
                countAl = 8;
            } else {
                p.ReadInt();
                countAl = 9;
            }

            _avatarLook = new int[countAl];
            for (int i = 0; i < countAl; i++) {
                _avatarLook[i] = p.ReadInt();
            }

            if (countAl == (int) CLogin.OnNewCharPacket22) {
                _gender = p.ReadByte();
            }

            return true;
        }

        public override void OnHandle() {
            var user = Client.User;
            user.AvatarLook.Face = _avatarLook[(int) AvatarSel.Face];
            user.AvatarLook.Hair = _avatarLook[(int) AvatarSel.Hair] + _avatarLook[(int) AvatarSel.HairColor];
            user.AvatarLook.Skin = (byte) _avatarLook[(int) AvatarSel.SkinColor];
            user.AvatarLook.Face = _avatarLook[(int) AvatarSel.Face];
            user.AvatarLook.Face = _avatarLook[(int) AvatarSel.Face];

            var inventory = user.Inventories[InventoryType.Equipped];
            inventory.EquipItem(new Equip(_avatarLook[(int) AvatarSel.Top], true));
            inventory.EquipItem(new Equip(_avatarLook[(int) AvatarSel.Bottom], true));
            inventory.EquipItem(new Equip(_avatarLook[(int) AvatarSel.Shoes], true));
            inventory.EquipItem(new Equip(_avatarLook[(int) AvatarSel.Weapon], true));

            user.CharacterStat.Id = 1;

            Client.Session.Write(GetCreateNewChar());
            Client.Users.Add(user);
        }

        private byte[] GetCreateNewChar() {
            using Packet p = new Packet();
            p.WriteShort((short) SendOps.CLogin.OnCreateNewCharacterResult);
            p.WriteByte();
            Client.User.CharacterStat.Encode(Client.User, p);
            Client.User.AvatarLook.Encode(Client.User, p);
            return p.ToArray();
        }

        /// <summary>
        /// <para>10    for "Could not be processed due to too many connection requests to the server."</para>
        /// <para>26    for "You cannot create a new character \r\nunder the account that \r\nhas requested for a transfer."</para>
        /// <para>30    for "You cannot use this name."</para>
        /// <para>1+    for "Failed due to unknown reason."</para>
        /// </summary>
        /// <param name="a">failure reason</param>
        private static byte[] GetCreateNewCharFailed(byte a) {
            using Packet p = new Packet();
            p.WriteShort((short) SendOps.CLogin.OnCreateNewCharacterResult);
            p.WriteByte(a);
            return p.ToArray();
        }
    }

    /// <summary>
    /// As listed in game files under <code>UI/Login.img/newChar*</code>
    /// <para>Called via <code>CLogin::GetSelectedAL</code></para>
    /// </summary>
    enum AvatarSel : int {
        Face = 0,
        Hair = 1,
        HairColor = 2,
        SkinColor = 3,
        Top = 4,
        Bottom = 5,
        Shoes = 6,
        Weapon = 7,
        Gender = 8,
        Outfit = 9, // only for NewCharResistance
    }
}