using System;
using NineToFive.Net;

namespace NineToFive.Game.Storage {
    public class Equip : Item {
        /// <param name="id">the id if the equip</param>
        /// <param name="autoBagIndex">should the BagIndex be automatically assigned to the associated body part</param>
        public Equip(int id, bool autoBagIndex = false) : base(id) {
            if (InventoryType != InventoryType.Equip) throw new InvalidOperationException("cannot create Equip of Item : " + id);
            if (autoBagIndex) BagIndex = (short) -ItemConstants.GetBodyPartFromId(id);
        }

        public override string ToString() {
            return $"Equip{{ID: {Id}, BagIndex: {BagIndex}}}";
        }

        public override byte Type => 1;

        public override ushort Quantity => 1;

        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public short MaxHP { get; set; }
        public short MaxMP { get; set; }
        public short PAD { get; set; }
        public short MAD { get; set; }
        public short PDD { get; set; }
        public short MDD { get; set; }
        public short Acc { get; set; }
        public short Eva { get; set; }
        public short Craft { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public string Title { get; set; } = "";
        public byte CHUC { get; set; }

        public override void Encode(Item item, Packet p) {
            if (!(item is Equip equip)) throw new NullReferenceException();
            base.Encode(equip, p);
            p.WriteByte(); // nRUC
            p.WriteByte(); // nCUC
            p.WriteShort(equip.Str);
            p.WriteShort(equip.Dex);
            p.WriteShort(equip.Int);
            p.WriteShort(equip.Luk);
            p.WriteShort(equip.MaxHP);
            p.WriteShort(equip.MaxMP);
            p.WriteShort(equip.PAD);
            p.WriteShort(equip.MAD);
            p.WriteShort(equip.PDD);
            p.WriteShort(equip.MDD);
            p.WriteShort(equip.Acc);
            p.WriteShort(equip.Eva);

            p.WriteShort(equip.Craft);
            p.WriteShort(equip.Speed);
            p.WriteShort(equip.Jump);
            p.WriteString(equip.Title);
            p.WriteShort();

            p.WriteByte(); // iLevelUpType
            p.WriteByte(); // iLevel
            p.WriteInt();  // iEXP
            p.WriteInt();  // iDurability

            p.WriteInt(); // nIUC

            p.WriteByte(); // nGrade
            p.WriteByte(equip.CHUC);
            p.WriteShort(); // nOption1
            p.WriteShort(); // nOption2
            p.WriteShort(); // nOption3
            p.WriteShort(); // nOption4/Socket1
            p.WriteShort(); // nOption5/Socket2
            if (equip.CashItemSn == 0) {
                p.WriteLong(equip.CashItemSn);
            }

            p.WriteLong(); // ftEquipped (file time?)
            p.WriteInt();  // nPrevBonusExpRate
        }

        public override void Decode(Item item, Packet p) { }
    }
}