const Libs         = Host.lib("mscorlib", "Common");
const Int16        = Libs.System.Int16;
const WzCache      = Libs.NineToFive.Resources.WzCache;
const SkillRecord  = Libs.NineToFive.Game.Entity.Meta.SkillRecord;
const CWvsPackets  = Libs.NineToFive.Packets.CWvsPackets;
const List         = Libs.System.Collections.Generic.List;
const JobConstants = Libs.NineToFive.JobConstants;

async function run() {
    let user = ctx.User;

    let maxAll = false;
    if(ctx.Args.Length > 0){
        if(ctx.Args[0] != "all"){
            user.SendMessage("[command] !maxskills or !maxskills all (to max all skills from all jobs)");
        } else {
            maxAll = true;
        }
    }

    let userSkills = user.Skills;
    userSkills.Clear();

    let list = new List(SkillRecord);
    let iterator =  WzCache.Skills.GetEnumerator();

    while(iterator.MoveNext()){
        let current = iterator.Current;
        let skillId = current.Key;
        let level = !maxAll && !JobConstants.CheckLineage(((skillId/10000) << 16) >> 16, user.CharacterStat.Job) ? 0 : current.Value.MaxLevel;
        let record = new SkillRecord(skillId, level);
            
        list.Add(record);
        userSkills.TryAdd(skillId, record);
    }

    user.Client.Session.Write(CWvsPackets.GetChangeSkillRecordResult(list))
}