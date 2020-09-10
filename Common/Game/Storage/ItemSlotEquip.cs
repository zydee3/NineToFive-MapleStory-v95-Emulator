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
        public byte Vicious { get; set; }
        public byte Upgrades { get; set; }
        
        public override byte Type => 1;

        public ItemSlotEquip(int itemId, bool autoBagIndex = false, bool loadStats = false) : base(itemId) {
            if (InventoryType != InventoryType.Equip) throw new InvalidOperationException($"cannot create Equip of Item : {TemplateId}");
            if (autoBagIndex) BagIndex = (short) -ItemConstants.GetBodyPartFromId(TemplateId);
            Durability = -1;
            Title ??= "";
            if(loadStats) CharacterWz.CopyEquipTemplate(this);
        }

        public override void Encode(Packet p) {
            base.Encode(p);
            p.WriteByte(); // nRUC
            p.WriteByte(); // nCUC
            p.WriteShort(STR);
            p.WriteShort(DEX);
            p.WriteShort(INT);
            p.WriteShort(LUK);
            
            p.WriteShort(MaxHP);
            p.WriteShort(MaxMP);
            
            p.WriteShort(PAD);
            p.WriteShort(MAD);
            p.WriteShort(PDD);
            p.WriteShort(MDD);
            p.WriteShort(ACC);
            p.WriteShort(EVA);

            p.WriteShort(Craft);
            p.WriteShort(Speed);
            p.WriteShort(Jump);
            p.WriteString(Title);
            p.WriteShort(Attribute);

            p.WriteByte(LevelUpType); // iLevelUpType
            p.WriteByte(Level); // iLevel
            p.WriteInt(Exp);  // iEXP
            p.WriteInt(Durability);  // iDurability

            p.WriteInt(IUC); // nIUC

            p.WriteByte(Grade); // nGrade
            p.WriteByte(CHUC);
            
            p.WriteShort(Option1); // nOption1
            p.WriteShort(Option2); // nOption2
            p.WriteShort(Option3); // nOption3
            p.WriteShort(Socket1); // nOption4/Socket1
            p.WriteShort(Socket2); // nOption5/Socket2
            if (CashItemSN == 0) {
                p.WriteLong(CashItemSN);
            }

            p.WriteLong(FTEquipped); // ftEquipped (file time?)
            p.WriteInt(PrevBonusExpRate);  // nPrevBonusExpRate
        }
    }
}