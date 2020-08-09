const Libs = Host.lib('Common');
const EntityType = Libs.NineToFive.Constants.EntityType;

async function run() {
    let user = Ctx.User;

    let pools = user.Field.LifePools.GetEnumerator();
    while (pools.MoveNext()) {
        if (pools.Current.Key != EntityType.Drop) continue;

        let drops = pools.Current // KeyValuePair<TKey,TValue>
            .Value // LifePool
            .Values.GetEnumerator(); // ValueCollection

        while (drops.MoveNext()) {
            let drop = drops.Current;
            user.Field.RemoveLife(drop);
        }
        break;
    }
    user.SendMessage("Drops cleared.")
}