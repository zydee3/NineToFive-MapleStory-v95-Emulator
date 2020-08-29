using System.Collections.Concurrent;
using System.Timers;
using NineToFive.Game.Entity;

namespace NineToFive.Game {
    public class World {
        public World(byte id) {
            Id = id;
        }

        public static byte ActiveWorld { get; set; }

        public byte Id { get; }
        public string Name => ServerConstants.WorldNames[Id];
        public Channel[] Channels { get; internal set; }
        public ConcurrentDictionary<uint, User> Users { get; } = new ConcurrentDictionary<uint, User>();
        public Timer UpdateFieldTimer { get; set; }

        public void DoUpdateFields(object o, ElapsedEventArgs e) {
            foreach (Channel channel in Channels) {
                foreach (Field field in channel.Fields.Values) {
                    field.Update(channel).ConfigureAwait(false);
                }
            }
        }
    }
}