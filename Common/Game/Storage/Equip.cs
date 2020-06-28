using NineToFive.IO;

namespace NineToFive.Game.Storage {
    public class Equip : Item {
        public Equip(int id) : base(id) { }

        public override void Encode(Item item, Packet p) {
            base.Encode(item, p);
        }

        public override void Decode(Item item, Packet p) {
            base.Decode(item, p);
        }
    }
    
    public enum EquipType : short {
        CAP = -1,
        FACE = -2,
        EYE = -3,
        EARRINGS = -4,
        OVERALL = -5,
        TOP = -5,
        BOTTOM = -6,
        SHOES = -7,
        GLOVES = -8,
        CAPE = -9,
        SHIELD = -10,
        WEAPON = -11,
        RING1 = -12,
        RING2 = -13,
        RING3 = -15,
        RING4 = -16,
        PENDANT = -17,
        MOUNT = -18,
        SADDLE = -19,
    }
}