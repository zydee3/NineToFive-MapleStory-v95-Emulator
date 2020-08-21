async function run() {
    switch (ctx.Status) {
        case 0: return ctx.SendSay("Hello!");
        case 1: return ctx.SendSay("World!");
    }    
}