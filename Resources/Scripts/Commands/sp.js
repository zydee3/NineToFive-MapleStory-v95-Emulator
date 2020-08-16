const Libs = Host.lib("mscorlib", "Common");
const Int16 = Libs.System.Int16;

async function run() {
    let user = ctx.User;
    if (ctx.Args.Length != 1) {
        return user.SendMessage("[command] !sp <new amount>");
    }
    let n = parseInt(ctx.Args[0]);
    if (isNaN(n) || n < 0 || n > 32767) {
        return user.SendMessage(`'${ctx.Args[0]}' is either too high, or too low.`)
    }

    user.CharacterStat.SP = ctx.ArgAsInt(0);
    user.CharacterStat.SendUpdate(user, 0x8000)
    user.SendMessage(`SP is now ${user.CharacterStat.SP[0]}`);
}