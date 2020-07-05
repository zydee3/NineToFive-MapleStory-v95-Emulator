using NineToFive.IO;
using System;
using log4net;

namespace NineToFive.Event {
    public class PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PacketEvent));
        public PacketEvent(Client client) {
            Client = client;
        }

        public Client Client { get; private set; }

        public virtual void OnError(Exception e) {
            Log.Error($"================ {GetType().Name} ================", e);
        }

        public virtual bool OnProcess(Packet p) {
            return false;
        }

        public virtual void OnHandle() {
        }
    }
}
