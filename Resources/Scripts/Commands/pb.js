const Libs = Host.lib('Common');
const Packet = Libs.NineToFive.Net.Packet;

async function run() {
    let user = ctx.User;
    if (ctx.Args.Length < 3) {
        return user.SendMessage("[command] !pb : not enough information");
    }

    let w = new Packet();
    try {
        for (let i = 1; i < ctx.Args.Length; i++) {
            w.WriteByte(ctx.ArgAsInt(i));
        }

        Console.WriteLine(w.ToArrayString(true));

        Client.Session.Write(w.ToArray());
    } finally {
        w.Dispose();
    }
}