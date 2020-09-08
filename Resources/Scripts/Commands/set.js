const nMaxStat = 32767;

async function run() {
    let user = ctx.User;

    if (ctx.Args.Length != 2) {
        return user.SendMessage("[command] !set <hp/mp/str/dex/int/luk/all> <amount>");
    }

    let stat = ctx.Args[0];
    let amount = parseInt(ctx.Args[1]);
    if(isNaN(amount) || amount < 0 || amount > nMaxStat){
        return user.SendMessage(`New amount needs to be between 0 and ${nMaxStat}.`);
    }

    switch(stat){
        case "hp":
            user.CharacterStat.HP = amount;
            user.CharacterStat.SendUpdate(0x400);
            break;   
        case "mp":
            user.CharacterStat.MP = amount;
            user.CharacterStat.SendUpdate(0x1000);
            break;
        case "str":
            user.CharacterStat.Str = amount;
            user.CharacterStat.SendUpdate(0x40);
            break;
        case "dex":
            user.CharacterStat.Dex = amount;
            user.CharacterStat.SendUpdate(0x80);
            break;
        case "int":
            user.CharacterStat.Int = amount;
            user.CharacterStat.SendUpdate(0x100);
            break;
        case "luk":
            user.CharacterStat.Luk = amount;
            user.CharacterStat.SendUpdate(0x200);
            break;
        case "all":
            user.CharacterStat.Str = amount;
            user.CharacterStat.Dex = amount;
            user.CharacterStat.Int = amount;
            user.CharacterStat.Luk = amount;
            user.CharacterStat.SendUpdate(0x40 | 0x80 | 0x100 | 0x200);
            break;
        default:
            return user.SendMessage("Invalid option. Options are hp, mp, str, dex, int, luk, and all.");
    }
}