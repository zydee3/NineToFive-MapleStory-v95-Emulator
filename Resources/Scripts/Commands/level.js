async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !level <new level>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n) || n < 0 || n > 255) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid level.`)
    }
    user.CharacterStat.Level = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(0x10);
    user.SendMessage(`Level is now ${user.CharacterStat.Level}`);
}