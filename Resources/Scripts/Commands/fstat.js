const Libs = Host.lib('Common');
const CWvsPackets = Libs.NineToFive.Packets.CWvsPackets;
const ForcedStatTypes = {
    'str': 1,
    'dex': 2,
    'int': 4,
    'luk': 8,
    'pad': 16,
    'pdd': 32,
    'mad': 64,
    'mdd': 128,
    'acc': 256,
    'eva': 512,
    'speed': 1024,
    'jump': 2048,
    'speedmax': 4096,
};

async function run() {
    let user = ctx.User;
    if (ctx.Args.Length == 0) {
        user.Client.Session.Write(CWvsPackets.GetForcedStatReset(user)); 
        user.SendMessage("Forced stat reset"); 
    } else if (ctx.Args.Length % 2 != 0) {
        user.SendMessage("[command] !fstat <(stat, value), [stat, value]...>");
        user.SendMessage(Object.keys(ForcedStatTypes).toString());
    } else {
        let flags = "";
        let dwcharFlags = 0;
        for (let i = 0; i < ctx.Args.Length; i += 2) {
            
            let n = ForcedStatTypes[ctx.Args[i].toLowerCase()];
            let v = parseInt(ctx.Args[i + 1]);

            if (n == undefined) return user.SendMessage(`'${ctx.Args[i]}' is an invalid flag.`);
            if (isNaN(v)) return user.SendMessage(`'${ctx.Args[i + 1]}' is an invalid value.`); 

            user.ForcedStat.SetByFlag(n, v);
            dwcharFlags |= n;
            flags += ctx.Args[i] + ", ";
        }
        flags = flags.substring(0, flags.length - 2);
        user.Client.Session.Write(CWvsPackets.GetForcedStatSet(user, dwcharFlags)); 
        user.SendMessage(`Forced stat${(ctx.Args.Length > 2 ? "s" : "")} applied : ${dwcharFlags} - [${flags}]`);
    }
}