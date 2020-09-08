using System;
using NineToFive.Net;

namespace NineToFive.Game.Storage {
    public class ItemSlotPet : ItemSlot {
        
        public string Name { get; set; }
        public short Skill { get; set; }
        public byte Repleteness { get; set; }
        public byte Level { get; set; }
        public short Tameness { get; set; }
        public int RemainLife { get; set; }
        public byte ActiveState { get; set; }
        public int AutoBuffSkill { get; set; }
        public int Hue { get; set; }
        public short GiantRate { get; set; }
        
        
        public override byte Type => 3;
        
        public ItemSlotPet(int templateId) : base(templateId) {
            
        }

        public override void Encode(Packet p) {
            base.Encode(p);
            p.WriteStringFixed(Name, 13);
            p.WriteByte(Level);
            p.WriteShort(Tameness);
            p.WriteByte(Repleteness);
            p.WriteLong(DateExpire);
            p.WriteShort(Attribute);
            p.WriteShort(Skill);
            p.WriteInt(RemainLife);
            p.WriteInt(ActiveState);
            p.WriteInt(AutoBuffSkill);
            p.WriteInt(Hue);
            p.WriteShort(GiantRate);
        }
    }
}