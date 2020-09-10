async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !mp <new mp>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n) || n < 0) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid value.`)
    }
    user.CharacterStat.MP = ctx.ArgAsInt(0);
    user.CharacterStat.MaxMP = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(0x1000 | 0x2000);
    user.SendMessage(`MP is now ${user.CharacterStat.MP}`);
}