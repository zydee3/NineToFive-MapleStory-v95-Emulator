const Libs        = Host.lib("mscorlib", "Common");
const Int16       = Libs.System.Int16;
const WzCache     = Libs.NineToFive.Resources.WzCache;
const SkillRecord = Libs.NineToFive.Game.Entity.Meta.SkillRecord;
const CWvsPackets = Libs.NineToFive.Packets.CWvsPackets;
const List        = Libs.System.Collections.Generic.List;

async function run() {
    let user = ctx.User;

    let userSkills = user.Skills;
    userSkills.Clear();

    let list = new List(SkillRecord);
    let iterator =  WzCache.Skills.GetEnumerator();

    while(iterator.MoveNext()){
        let current = iterator.Current;
        let skillId = current.Key;
        let skill = current.Value;
        
        let record = new SkillRecord(skillId, skill.MaxLevel);
        list.Add(record);
        userSkills.TryAdd(skillId, record);
    }

    user.Client.Session.Write(CWvsPackets.GetChangeSkillRecordResult(list))
}