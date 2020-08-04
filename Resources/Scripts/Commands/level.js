async function run() {
    let User = Ctx.User;
    if (Ctx.Args.Length != 1) {
        return User.SendMessage("[command] !level <new level>");
    }
    let n = parseInt(Ctx.Args[0]);
    if (isNaN(n) || n < 0 || n > 255) {
        return User.SendMessage(`'${Ctx.Args[0]}' is not a valid level.`)
    }
    User.CharacterStat.Level = Ctx.ArgAsInt(0);
    User.CharacterStat.SendUpdate(User, 0x10);
    User.SendMessage(`Level is now ${User.CharacterStat.Level}`);
}