const Libs    = Host.lib('Common');
const Drop      = Libs.NineToFive.Game.Entity.Drop;

async function run() {
    let User = Ctx.User;
    if (Ctx.Args.Length != 1) {
        return User.SendMessage("[command] !drop <item id>");
    }
    let n = parseInt(Ctx.Args[0]);
    if (isNaN(n)) {
        return User.SendMessage(`'${Ctx.Args[0]}' is not a valid number.`)
    }
    let drop = new Drop(Ctx.ArgAsInt(0), User);
    User.Field.SummonLife(drop);
}