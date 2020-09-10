async function run() {
    let user = ctx.User;
    user.CharacterStat.SendUpdate(0);
    user.SendMessage("Disposed.");
}