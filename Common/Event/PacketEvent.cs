using System;
using log4net;
using NineToFive.Net;

namespace NineToFive.Event {
    public static class Extensions {
        public static bool IsEqual(this byte[] buffer, byte[] other) {
            if (buffer.Length != other.Length) return false;
            for (int i = 0; i < buffer.Length; i++) {
                if (buffer[i] != other[i]) return false;
            }

            return true;
        }
    }

    public class PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PacketEvent));

        public PacketEvent(Client client) {
            Client = client;
        }

        public Client Client { get; }

        public virtual void OnError(Exception e) {
            Log.Error($"================ {GetType().Name} ================", e);
        }

        public virtual bool ShouldProcess() {
            return Client.LoginStatus != 0;
        }

        public virtual bool OnProcess(Packet p) {
            return false;
        }

        public virtual void OnHandle() { }
    }
}