async function run() {
    let User = Ctx.User;
    if (Ctx.Args.Length != 1) {
        return User.SendMessage("[command] !ap <new amount>");
    }
    let n = parseInt(Ctx.Args[0]);
    if (isNaN(n) || n < 0 || n > 32767) {
        return User.SendMessage(`'${Ctx.Args[0]}' is either too high, or too low.`)
    }
    User.CharacterStat.AP = Ctx.ArgAsInt(0);
    User.CharacterStat.SendUpdate(User, 0x4000);
    User.SendMessage(`AP is now ${User.CharacterStat.AP}`);
}