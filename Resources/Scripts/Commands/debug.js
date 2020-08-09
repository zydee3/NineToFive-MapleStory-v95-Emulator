async function run() {
    let user = ctx.User;
    user.IsDebugging = !user.IsDebugging;
    user.SendMessage(`Debug: ${(user.IsDebugging ? "Enabled" : "Disabled")}`);
}