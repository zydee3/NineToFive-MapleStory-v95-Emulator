async function run() {
    let user = ctx.User;
    user.CharacterStat.HP = 0;
    user.CharacterStat.SendUpdate(user, 0x400);    
}