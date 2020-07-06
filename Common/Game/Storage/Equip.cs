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

        public override byte Type => 1;

        public override ushort Quantity => 1;

        public override void Encode(Item item, Packet p) {
            base.Encode(item, p);
            p.WriteByte();
            p.WriteByte();
            p.WriteShort(); // iStr
            p.WriteShort(); // iDex
            p.WriteShort(); // iInt
            p.WriteShort(); // iLuk
            p.WriteShort(); // iMaxHp
            p.WriteShort(); // iMaxMp
            p.WriteShort(); // iPAD
            p.WriteShort(); // iMAD
            p.WriteShort(); // iPDD
            p.WriteShort(); // iMDD
            p.WriteShort(); // iACC
            p.WriteShort(); // iEVA
            p.WriteShort(); // iCraft
            p.WriteShort(); // iSpeed
            p.WriteShort(); // iJump
            p.WriteString(); // sTitle
            p.WriteShort();
            p.WriteByte();
            p.WriteByte();
            p.WriteInt();
            p.WriteInt();
            p.WriteInt();
            p.WriteByte(); // nGrade
            p.WriteByte(); // nCHUC
            p.WriteShort(); // nOption1
            p.WriteShort(); // nOption2
            p.WriteShort(); // nOption3
            p.WriteShort(); // nOption4
            p.WriteShort(); // nOption5
            // liCashItemSN.QuadPart
            if (CashItemSn > 0) {
                p.WriteLong(CashItemSn);
            }
            p.WriteLong(); // ftEquipped (file time?)
            p.WriteInt(); // nPrevBonusExpRate
        }

        public override void Decode(Item item, Packet p) {
            base.Decode(item, p);
        }
    }
}