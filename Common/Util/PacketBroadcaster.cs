using System.Collections.Generic;
using System.Linq;

namespace NineToFive.Util {
    public abstract class PacketBroadcaster {
        public abstract IEnumerable<Client> GetClients();

        public void Broadcast(byte[] a) {
            // a client Id cannot be 0
            BroadcastSkip(a, 0);
        }

        public void BroadcastSkip(byte[] a, uint clientId) {
            foreach (var client in GetClients().Where(c => c.Id != clientId)) {
                client.Session.Write(a);
            }
        }
    }
}