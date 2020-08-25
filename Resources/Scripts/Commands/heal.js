async function run() {
    let user = ctx.User;
    user.CharacterStat.HP = user.CharacterStat.MaxHP;
    user.CharacterStat.MP = user.CharacterStat.MaxMP;
    user.CharacterStat.SendUpdate(user, (0x400 | 0x1000));    
}