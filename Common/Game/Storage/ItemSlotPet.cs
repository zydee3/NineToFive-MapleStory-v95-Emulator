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

        public override void Encode(ItemSlot itemSlot, Packet p) {
            if(!(itemSlot is ItemSlotPet pet)) throw new InvalidOperationException();
            base.Encode(pet, p);
            p.WriteStringFixed(pet.Name, 13);
            p.WriteByte(pet.Level);
            p.WriteShort(pet.Tameness);
            p.WriteByte(pet.Repleteness);
            p.WriteLong(pet.DateExpire);
            p.WriteShort(pet.Attribute);
            p.WriteShort(pet.Skill);
            p.WriteInt(pet.RemainLife);
            p.WriteInt(pet.ActiveState);
            p.WriteInt(pet.AutoBuffSkill);
            p.WriteInt(pet.Hue);
            p.WriteShort(pet.GiantRate);
        }
    }
}