using System;
using log4net;
using NineToFive.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;

namespace NineToFive.Channels {
    class ChannelServer : ServerListener {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChannelServer));
        private RecvOps Receive { get; }

        public ChannelServer(int port) : base(port) {
            Receive = new RecvOps();
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!Receive.Events.TryGetValue(operation, out Type t)) {
                Log.Info($"Unhandled operation {operation}");
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                Log.Info($"{operation} (0x{operation:X2}) {p.ToArrayString(true)}");
                Log.Info(p.ToString());
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