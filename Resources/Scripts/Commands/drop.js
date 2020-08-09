const Libs    = Host.lib('Common');
const Drop    = Libs.NineToFive.Game.Entity.Drop;

async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !drop <item id>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n)) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid number.`)
    }
    let drop = new Drop(ctx.ArgAsInt(0), user);
    user.Field.SummonLife(drop);
}