using System;
using System.ComponentModel;

namespace NineToFive.Game.Entity.Meta {
    public static class Extensions {
        public static int GetFromSkill(this TemporaryStat stat, Skill skill, SkillRecord record) {
            switch (stat) {
                case TemporaryStat.None:
                    break;
                case TemporaryStat.PAD:
                    return skill.PAD[record.Level - 1];
                case TemporaryStat.PDD:
                    return skill.PDD[record.Level - 1];
                case TemporaryStat.MAD:
                    return skill.MAD[record.Level - 1];
                case TemporaryStat.MDD:
                    return skill.MDD[record.Level - 1];
                case TemporaryStat.Acc:
                    return skill.Acc[record.Level - 1];
                case TemporaryStat.Eva:
                    return skill.Eva[record.Level - 1];
                case TemporaryStat.Hands:
                    break;
                case TemporaryStat.Speed:
                    return skill.Speed[record.Level - 1];
                case TemporaryStat.Jump:
                    return skill.Jump[record.Level - 1];
                case TemporaryStat.Ghost:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }

            throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
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
                case UserAbility.Exp:        return user.CharacterStat.Exp;
                case UserAbility.Popularity: return user.CharacterStat.Popularity;
                case UserAbility.FieldId:    return user.CharacterStat.FieldId;
                case UserAbility.Money:      return (int) user.Money;
            }

            throw new InvalidEnumArgumentException("unknown ability: " + ability);
        }
    }
}