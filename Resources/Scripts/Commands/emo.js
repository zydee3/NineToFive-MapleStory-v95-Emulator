async function run() {
    let user = ctx.User;
    user.CharacterStat.HP = 0;
    user.CharacterStat.SendUpdate(0x400);    
}