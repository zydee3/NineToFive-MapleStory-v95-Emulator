async function run() {
    let user = Ctx.User;
    user.SendMessage(`Pos${user.Location.ToString()} , Vel${user.Velocity.ToString()}`);
}