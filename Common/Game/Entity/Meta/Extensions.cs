using System.ComponentModel;

namespace NineToFive.Game.Entity.Meta {
    public static class Extensions {
        public static bool IsCalcDamageStat(this SecondaryStat stat) {
            return stat switch {
                SecondaryStat.MAD              => true,
                SecondaryStat.ACC              => true,
                SecondaryStat.Darkness         => true,
                SecondaryStat.ComboCounter     => true,
                SecondaryStat.WeaponCharge     => true,
                SecondaryStat.BasicStatUp      => true,
                SecondaryStat.SharpEyes        => true,
                SecondaryStat.MaxLevelBuff     => true,
                SecondaryStat.EnergyCharge     => true,
                SecondaryStat.ComboAbilityBuff => true,
                SecondaryStat.AssistCharge     => true,
                SecondaryStat.SuddenDeath      => true,
                SecondaryStat.FinalCut         => true,
                SecondaryStat.ThornsEffect     => true,
                SecondaryStat.EPAD             => true,
                SecondaryStat.DarkAura         => true,
                SecondaryStat.DamR             => true,
                SecondaryStat.BlessingArmor    => true,
                _                              => false
            };
        }

        public static bool IsMovementAffectingStat(this SecondaryStat stat) {
            return stat switch {
                SecondaryStat.Jump        => true,
                SecondaryStat.Speed       => true,
                SecondaryStat.Stun        => true,
                SecondaryStat.Weakness    => true,
                SecondaryStat.Slow        => true,
                SecondaryStat.Morph       => true,
                SecondaryStat.Ghost       => true,
                SecondaryStat.BasicStatUp => true,
                SecondaryStat.Attract     => true,
                SecondaryStat.RideVehicle => true,
                SecondaryStat.DashSpeed   => true,
                SecondaryStat.DashJump    => true,
                SecondaryStat.Flying      => true,
                SecondaryStat.Frozen      => true,
                SecondaryStat.YellowAura  => true,
                _                         => false
            };
        }

        public static int GetFromUser(this UserAbility ability, User user) {
            switch (ability) {
                case UserAbility.Skin:       return user.AvatarLook.Skin;
                case UserAbility.Face:       return user.AvatarLook.Face;
                case UserAbility.Hair:       return user.AvatarLook.Hair;
                case UserAbility.Level:      return user.CharacterStat.Level;
                case UserAbility.Job:        return user.CharacterStat.Job;
                case UserAbility.Str:        return user.CharacterStat.Str;
                case UserAbility.Dex:        return user.CharacterStat.Dex;
                case UserAbility.Int:        return user.CharacterStat.Int;
                case UserAbility.Luk:        return user.CharacterStat.Luk;
                case UserAbility.HP:         return user.CharacterStat.HP;
                case UserAbility.MaxHP:      return user.CharacterStat.MaxHP;
                case UserAbility.MP:         return user.CharacterStat.MP;
                case UserAbility.MaxMP:      return user.CharacterStat.MaxMP;
                case UserAbility.AP:         return user.CharacterStat.AP;
                case UserAbility.Exp:        return (int) user.CharacterStat.Exp;
                case UserAbility.Popularity: return user.CharacterStat.Popularity;
                case UserAbility.FieldId:    return user.CharacterStat.FieldId;
                case UserAbility.Money:      return (int) user.Money;
            }

            throw new InvalidEnumArgumentException("unknown ability: " + ability);
        }
    }
}