const Libs    = Host.lib('Common');
const Drop    = Libs.NineToFive.Game.Entity.Drop;

async function run() {
    let user = ctx.User;
    let nArgs = ctx.Args.Length;
    if (nArgs != 1 && nArgs != 2) {
        return user.SendMessage("[command] !drop <item id> <quantity>");
    }

    let n = parseInt(ctx.Args[0]);
    let quantity = (nArgs == 1 ? 1 : parseInt(ctx.Args[1]));

    if (isNaN(n)) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a valid number.`)
    } else if (isNaN(quantity)){
        return user.SendMessage(`'${ctx.Args[1]}' is not a valid number.`)
    }

    let drop = new Drop(ctx.ArgAsInt(0), quantity, user.Location, user.Location);
    user.Field.SummonLife(drop);
}