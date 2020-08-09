async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !map <map id>");
    }
    let fieldId = parseInt(ctx.Args[0]);
    if (isNaN(fieldId)) return user.SendMessage(`Invalid map : '${ctx.Args[0]}'`);
    user.SetField(ctx.ArgAsInt(0));
}