using System;
using NineToFive.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;

namespace NineToFive.Channels {
    class ChannelServer : ServerListener {
        private RecvOps Receive { get; }

        public ChannelServer(int port) : base(port) {
            Receive = new RecvOps();
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!Receive.Events.TryGetValue(operation, out Type t)) {
                Console.WriteLine("[LoginServer] Unhandled operation {0}", operation);
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                Console.WriteLine("[LoginServer] {0} (0x{1}) {2}", operation, operation.ToString("X2"), p.ToArrayString(true));
                Console.WriteLine(p.ToString());
                try {
                    if (handler.OnProcess(p)) {
                        handler.OnHandle();
                    }
                } catch (Exception e) {
                    handler.OnError(e);
                }
            }

            Console.WriteLine();
        }
    }
}