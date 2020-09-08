using System;
using NineToFive.Net;
using NineToFive.Wz;

namespace NineToFive.Game.Storage {
    public class ItemSlotEquip : ItemSlot {
        
        public byte RUC { get; set; }
        public byte CUC { get; set; }
        
        public short STR { get; set; }
        public short DEX { get; set; }
        public short INT { get; set; }
        public short LUK { get; set; }
        
        public short MaxHP { get; set; }
        public short MaxMP { get; set; }
        
        public short MaxHPR { get; set; }
        public short MaxMPR { get; set; }
        
        public short PAD { get; set; }
        public short MAD { get; set; }
        public short PDD { get; set; }
        public short MDD { get; set; }
        public short ACC { get; set; }
        public short EVA { get; set; }
        
        public short Craft { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public string Title { get; set; }
        
        public byte LevelUpType { get; set; }
        public byte Level { get; set; }
        public int Exp { get; set; }
        public int Durability { get; set; }
        
        public int IUC { get; set; }
        
        public byte Grade { get; set; }
        public byte CHUC { get; set; }
        
        public short RMAF { get; set; }
        public short RMAS { get; set; }
        public short RMAI { get; set; }
        public short RMAL { get; set; }

        public short Option1 { get; set; }
        public short Option2 { get; set; }
        public short Option3 { get; set; }
        public short Socket1 { get; set; }
        public short Socket2 { get; set; }
        
        public long FTEquipped { get; set; }
        public int PrevBonusExpRate { get; set; }
        
        public int TradeAvailable { get; set; }
        
        public override byte Type => 1;

        public ItemSlotEquip(int itemId, bool autoBagIndex = false, bool loadStats = false) : base(itemId) {
            if (InventoryType != InventoryType.Equip) throw new InvalidOperationException($"cannot create Equip of Item : {TemplateId}");
            if (autoBagIndex) BagIndex = (short) -ItemConstants.GetBodyPartFromId(TemplateId);
            Durability = -1;
            Title ??= "";
            CharacterWz.CopyEquipTemplate(this);
        }

        public override void Encode(ItemSlot itemSlot, Packet p) {
            if(!(itemSlot is ItemSlotEquip equip)) throw new NullReferenceException();
            base.Encode(equip, p);
            p.WriteByte(); // nRUC
            p.WriteByte(); // nCUC
            p.WriteShort(equip.STR);
            p.WriteShort(equip.DEX);
            p.WriteShort(equip.INT);
            p.WriteShort(equip.LUK);
            
            p.WriteShort(equip.MaxHP);
            p.WriteShort(equip.MaxMP);
            
            p.WriteShort(equip.PAD);
            p.WriteShort(equip.MAD);
            p.WriteShort(equip.PDD);
            p.WriteShort(equip.MDD);
            p.WriteShort(equip.ACC);
            p.WriteShort(equip.EVA);

            p.WriteShort(equip.Craft);
            p.WriteShort(equip.Speed);
            p.WriteShort(equip.Jump);
            p.WriteString(equip.Title);
            p.WriteShort(equip.Attribute);

            p.WriteByte(equip.LevelUpType); // iLevelUpType
            p.WriteByte(equip.Level); // iLevel
            p.WriteInt(equip.Exp);  // iEXP
            p.WriteInt(equip.Durability);  // iDurability

            p.WriteInt(equip.IUC); // nIUC

            p.WriteByte(equip.Grade); // nGrade
            p.WriteByte(equip.CHUC);
            
            p.WriteShort(equip.Option1); // nOption1
            p.WriteShort(equip.Option2); // nOption2
            p.WriteShort(equip.Option3); // nOption3
            p.WriteShort(equip.Socket1); // nOption4/Socket1
            p.WriteShort(equip.Socket2); // nOption5/Socket2
            if (equip.CashItemSN == 0) {
                p.WriteLong(equip.CashItemSN);
            }

            p.WriteLong(equip.FTEquipped); // ftEquipped (file time?)
            p.WriteInt(equip.PrevBonusExpRate);  // nPrevBonusExpRate
        }
    }
}