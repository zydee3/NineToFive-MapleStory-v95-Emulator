async function run() {
    let user = ctx.User;
    let amount;

    if (ctx.Args.Length != 1 || isNaN(amount = parseInt(ctx.Args[0]))) {
        user.SendMessage("Enter a number stoopid head");
    }
   
    if(!user.GainMoney(amount)){
        user.sendMessage("Something went wrong");
    }
}