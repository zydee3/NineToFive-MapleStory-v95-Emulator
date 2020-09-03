async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        user.SendMessage(`Current job : ${user.CharacterStat.Job}`);
        return user.SendMessage("[command] !job <new job>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n)) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid number.`);
    }
    user.CharacterStat.Job = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(user, 0x20);
    user.SendMessage(`Job is now ${user.CharacterStat.Job}`);
}