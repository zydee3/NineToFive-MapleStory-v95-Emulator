const Libs         = Host.lib("mscorlib", "Common");
const Int16        = Libs.System.Int16;
const WzCache      = Libs.NineToFive.Resources.WzCache;
const SkillRecord  = Libs.NineToFive.Game.Entity.Meta.SkillRecord;
const CWvsPackets  = Libs.NineToFive.Packets.CWvsPackets;
const List         = Libs.System.Collections.Generic.List;
const JobConstants = Libs.NineToFive.JobConstants;

async function run() {
    let user = ctx.User;

    let userSkills = user.Skills;
    userSkills.Clear();

    let list = new List(SkillRecord);
    let iterator =  WzCache.Skills.GetEnumerator();

    while(iterator.MoveNext()){
        let current = iterator.Current;
        let skillId = current.Key;
        
        if(JobConstants.CheckLineage(((skillId/10000) << 16) >> 16, user.CharacterStat.Job)){           
            let record = new SkillRecord(skillId, current.Value.MaxLevel);
            list.Add(record);
            userSkills.TryAdd(skillId, record);
        }
    }

    user.Client.Session.Write(CWvsPackets.GetChangeSkillRecordResult(list))
}