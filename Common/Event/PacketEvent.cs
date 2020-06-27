using NineToFive.IO;
using System;

namespace NineToFive.Event {
    public class PacketEvent {

        public Client Client { get; private set; }

        public PacketEvent(Client client) {
            Client = client;
        }

        public virtual void OnError(Exception e) {
            Console.Write(e);
        }

        public virtual bool OnProcess(Packet p) {
            return false;
        }

        public virtual void OnHandle() {
        }
    }
}
