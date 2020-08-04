async function run() {
    let User = Ctx.User;
    User.IsDebugging = !User.IsDebugging;
    User.SendMessage(`Debug: ${(User.IsDebugging ? "Enabled" : "Disabled")}`);
}