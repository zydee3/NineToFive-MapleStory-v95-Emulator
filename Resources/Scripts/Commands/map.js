async function run() {
    let User = Ctx.User;
    if (Ctx.Args.Length != 1) {
        User.SendMessage("[command] !map <map id>");
        return;
    }
    let fieldId = parseInt(Ctx.Args[0]);
    if (isNaN(fieldId)) return User.SendMessage(`Invalid map : '${Ctx.Args[0]}'`);
    User.SetField(Ctx.ArgAsInt(0));
}