async function run() {
    let user = Ctx.User;
    if (Ctx.Args.Length != 1) {
        user.SendMessage("[command] !map : not enough arguments");
        return;
    }
    let fieldId = parseInt(Ctx.Args[0]);
    if (isNaN(fieldId)) return user.SendMessage(`Invalid map : '${Ctx.Args[0]}'`);
    user.SetField(Convert.ToInt32(Ctx.Args[0]));
}