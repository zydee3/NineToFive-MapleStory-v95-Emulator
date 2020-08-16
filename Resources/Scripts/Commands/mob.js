const Libs = Host.lib('Common');
const Mob = Libs.NineToFive.Game.Entity.Mob; 

async function run() {
    let user = ctx.User;

    let n = parseInt(ctx.Args[0]);
    if (isNaN(n)) {
        return user.SendMessage(`'${ctx.Args[0]}' is not a number.`)
    }
    let mob = new Mob(ctx.ArgAsInt(0));
    mob.Location = user.Location;
    mob.Fh = user.Fh;
    user.Field.SummonLife(mob);
    user.SendMessage(`Spawning mob {ID: ${mob.Id}, TemplateID: ${mob.TemplateId}}`);
}