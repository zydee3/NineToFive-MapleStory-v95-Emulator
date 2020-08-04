async function run() {
    let User = Ctx.User;
    User.CharacterStat.SendUpdate(User, 0);
    User.SendMessage("Disposed.");
}