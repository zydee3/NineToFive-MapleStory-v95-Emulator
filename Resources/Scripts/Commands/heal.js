async function run() {
    let user = ctx.User;
    user.CharacterStat.HP = user.CharacterStat.MaxHP;
    user.CharacterStat.SendUpdate(user, 0x400);    
}