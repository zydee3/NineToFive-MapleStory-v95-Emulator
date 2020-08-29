using System;
using log4net;
using log4net.Config;
using NineToFive.Event;
using NineToFive.Net;
using NineToFive.Net.Interoperations;

[assembly: XmlConfigurator(ConfigFile = "logger-config.xml")]

namespace NineToFive {
    public class LoginServer : ServerListener {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginServer));
        private readonly EventDirector _director;

        public LoginServer(int port) : base(port) {
            _director = new EventDirector {
                [(short) ReceiveOperations.ClientSocket_OnAliveReq] = typeof(KeepAliveEvent),
                [(short) ReceiveOperations.Login_OnCheckPasswordResult] = typeof(CheckPasswordEvent),
                [(short) ReceiveOperations.Login_OnWorldListReinitializeRequest] = typeof(WorldListEvent),
                [(short) ReceiveOperations.Login_OnChannelSelectEnterChannel] = typeof(SelectEnterChannelEvent),
                [(short) ReceiveOperations.Login_OnCheckUserLimitPacket] = typeof(CheckUserLimitEvent),
                [(short) ReceiveOperations.Login_OnLicenseResult] = typeof(LicenseResultEvent),
                [(short) ReceiveOperations.Login_OnSetGenderPacket] = typeof(SetGenderEvent),
                [(short) ReceiveOperations.Login_OnPinCodeResult] = typeof(PinCodeResultEvent),
                [(short) ReceiveOperations.Login_OnWorldListRequest] = typeof(WorldListEvent),
                [(short) ReceiveOperations.Login_OnViewAllCharPacket] = typeof(ViewAllCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnViewAllCharDlgResult] = typeof(ViewAllCharDlgEvent),
                [(short) ReceiveOperations.Login_OnCheckDuplicateIdPacket] = typeof(CheckDuplicateUsernameEvent),
                [(short) ReceiveOperations.Login_OnNewCharPacket22] = typeof(NewCharEvent),
                [(short) ReceiveOperations.Login_OnNewCharPacket23] = typeof(NewCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharInitSPWPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnSelectCharSPWPacket] = typeof(SelectCharEvent),
                [(short) ReceiveOperations.Login_OnSendBackupPacket] = typeof(BackupPacketEvent),
                [(short) ReceiveOperations.CWvsContext_OnUiMenu] = typeof(CWvsUiMenuEvent),
            };
        }

        public override void OnPacketReceived(Client c, Packet p) {
            short operation = p.ReadShort();
            if (!_director.Events.TryGetValue(operation, out Type t)) {
                Log.Debug($"[unhandled] {operation} (0x{operation:X2}) : {p.ToArrayString(true)}");
                Log.Debug($"[ascii-decode] {p}");
                Log.Debug("-----------");
                return;
            }

            object instance = Activator.CreateInstance(t, c);
            if (instance is PacketEvent handler) {
                try {
                    if (!handler.ShouldProcess()) {
                        p.Dispose();
                        return;
                    }

                    if (handler.OnProcess(p)) {
                        handler.OnHandle();
                    }
                } catch (Exception e) {
                    handler.OnError(e);
                }
            }
        }

        public static void Main(string[] args) {
            Log.Info("Hello World, from Login Server!");
            Interoperability.ServerCreate(ServerConstants.InterLoginPort);
            Log.Info($"Interoperations listening on port {ServerConstants.InterLoginPort}");

            // Initialize the login server socket
            LoginServer server = new LoginServer(ServerConstants.LoginPort);
            server.Start();
            Log.Info($"Listening on port {server.Port}");
        }
    }
}