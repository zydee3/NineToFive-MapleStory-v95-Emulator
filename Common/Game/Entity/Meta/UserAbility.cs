using System.ComponentModel;

namespace NineToFive.Game.Entity.Meta {
    public static class Extensions {
        public static int GetFromUser(this UserAbility ability, User user) {
            switch (ability) {
                case UserAbility.Skin: return user.AvatarLook.Skin;
                case UserAbility.Face: return user.AvatarLook.Face;
                case UserAbility.Hair: return user.AvatarLook.Hair;
                case UserAbility.Level: return user.CharacterStat.Level;
                case UserAbility.Job: return user.CharacterStat.Job;
                case UserAbility.Str: return user.CharacterStat.Str;
                case UserAbility.Dex: return user.CharacterStat.Dex;
                case UserAbility.Int: return user.CharacterStat.Int;
                case UserAbility.Luk: return user.CharacterStat.Luk;
                case UserAbility.HP: return user.CharacterStat.HP;
                case UserAbility.MaxHP: return user.CharacterStat.MaxHP;
                case UserAbility.MP: return user.CharacterStat.MP;
                case UserAbility.MaxMP: return user.CharacterStat.MaxMP;
                case UserAbility.AP: return user.CharacterStat.AP;
                case UserAbility.Exp: return user.CharacterStat.Exp;
                case UserAbility.Popularity: return user.CharacterStat.Popularity;
                case UserAbility.FieldId: return user.CharacterStat.FieldId;
                case UserAbility.Money: return (int) user.Money;
            }
            throw new InvalidEnumArgumentException("unknown ability: " + ability);
        }
    }
    
    public enum UserAbility {
        Skin = 1,
        Face = 4,
        Hair = 2,
        PetA = 8,
        PetB = 0x80000,
        PetC = 0x100000,
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
        FieldId = 0x200000,
    }
}