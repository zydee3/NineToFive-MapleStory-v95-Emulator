async function run() {
    let user = ctx.User;
    user.SendMessage(`Pos${user.Location.ToString()} , Vel${user.Velocity.ToString()}`);
}