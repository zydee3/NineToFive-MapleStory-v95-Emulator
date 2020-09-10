const Libs = Host.lib('Common');
const SkillWz = Libs.NineToFive.Wz.SkillWz; 

async function run() {
    let user = ctx.User;
    let count = SkillWz.LoadSkills();
    return user.SendMessage(`${count} skills reloaded.`);
}