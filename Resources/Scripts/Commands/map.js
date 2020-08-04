async function run() {
    let User = Ctx.User;
    if (Ctx.Args.Length != 1) {
        return User.SendMessage("[command] !map <map id>");
    }
    let fieldId = parseInt(Ctx.Args[0]);
    if (isNaN(fieldId)) return User.SendMessage(`Invalid map : '${Ctx.Args[0]}'`);
    User.SetField(Ctx.ArgAsInt(0));
}