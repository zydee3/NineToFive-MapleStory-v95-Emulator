namespace NineToFive.Constants {
    public static class ETExtensions {
        public static bool IsTemplate(this EntityType type) {
            switch (type) {
                case EntityType.Npc:
                case EntityType.Mob:
                case EntityType.Reactor:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum EntityType {
        Npc,
        Mob,
        User,
        Reactor,
        Summon,
        Pet
    }
}