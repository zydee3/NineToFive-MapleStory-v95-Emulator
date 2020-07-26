async function run() {
    let user = Ctx.User;
    user.IsDebugging = !user.IsDebugging;
    user.SendMessage(`Debug: ${(user.IsDebugging ? "Enabled" : "Disabled")}`);
}