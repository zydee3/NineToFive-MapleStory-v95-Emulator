async function run() {
    let user = ctx.User;
    user.CharacterStat.SendUpdate(user, 0);
    user.SendMessage("Disposed.");
}