namespace NineToFive.Game {
    public class World {
        public byte Id { get; }
        public Channel[] Channels { get; internal set; }
        public string Name => Constants.WorldNames[Id];

        public World(byte Id) {
            this.Id = Id;
        }
    }
}