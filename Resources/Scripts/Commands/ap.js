async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !ap <new amount>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n) || n < 0 || n > 32767) {
        return user.SendMessage(`'${ctx.Args[0]}' is either too high, or too low.`)
    }
    user.CharacterStat.AP = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(0x4000);
    user.SendMessage(`AP is now ${user.CharacterStat.AP}`);
}