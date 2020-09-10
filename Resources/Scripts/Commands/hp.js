async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !hp <new hp>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n) || n < 0) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid number.`)
    }
    user.CharacterStat.HP = ctx.ArgAsInt(0);
    user.CharacterStat.MaxHP = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(0x400 | 0x800);
    user.SendMessage(`HP is now ${user.CharacterStat.HP}`);
}