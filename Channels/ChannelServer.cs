using System;
using log4net;
using NineToFive.Channels.Event;
using NineToFive.Event;
using NineToFive.IO;
using NineToFive.Net;
using NineToFive.ReceiveOps;
using NineToFive.SendOps;
using CLogin = NineToFive.ReceiveOps.CLogin;

namespace NineToFive.Channels {
    class ChannelServer : ServerListener {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChannelServer));
        private readonly RecvOps _receive;

        public ChannelServer(int port) : base(port) {
            _receive = new RecvOps() {
                [(short) CLogin.OnEnterGamePacket] = typeof(CharEnterGameEvent),
            };
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!_receive.Events.TryGetValue(operation, out Type t)) {
                Console.WriteLine($"[unhandled] {operation} (0x{operation:X2}) : {p.ToArrayString(true)}");
                Console.WriteLine($"[ascii-decode] {p}");
                Console.WriteLine("-----------");
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                Log.Info($"Handled operation : {operation} | {operation:X2} : {handler.GetType().Name}");
                try {
                    if (handler.OnProcess(p)) {
                        handler.OnHandle();
                    }
                } catch (Exception e) {
                    handler.OnError(e);
                }
            }
        }
    }
}