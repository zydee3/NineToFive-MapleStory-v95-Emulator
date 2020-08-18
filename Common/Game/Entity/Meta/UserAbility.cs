using NineToFive.Net;

namespace NineToFive.Game.Entity.Meta {
    public enum UserAbility {
        Skin = 1,
        Hair = 2,
        Face = 4,
        PetA = 8,
        Level = 0x10,
        Job = 0x20,
        Str = 0x40,
        Dex = 0x80,
        Int = 0x100,
        Luk = 0x200,
        HP = 0x400,
        MaxHP = 0x800,
        MP = 0x1000,
        MaxMP = 0x2000,
        AP = 0x4000,
        SP = 0x8000,
        Exp = 0x10000,
        Popularity = 0x20000,
        Money = 0x40000,
        PetB = 0x80000,
        PetC = 0x100000,
        FieldId = 0x200000,
    }
}