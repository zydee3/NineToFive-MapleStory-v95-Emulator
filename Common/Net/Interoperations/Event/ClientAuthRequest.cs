using log4net;
using NineToFive.IO;

namespace NineToFive.Interopation.Event {
    public static class ClientAuthRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientAuthRequest));

        public static byte[] OnHandle(Packet r) {
            string username = r.ReadString();
            string password = r.ReadString();

            CentralServer.Clients.TryGetValue(username, out Client client);
            if (client == null) {
                client = new Client(null, null) {Username = username};
            }

            byte loginResult = client.TryLogin(password);
            using Packet w = new Packet();
            w.WriteByte(loginResult);
            if (loginResult == 1) {
                w.WriteInt(client.Id);
                w.WriteByte(client.Gender);
                if (w.WriteBool(client.SecondaryPassword != null)) {
                    w.WriteString(client.SecondaryPassword);
                }

                Log.Info($"{client.Username} has logged-in");
                CentralServer.Clients.TryAdd(username, client);
            }

            return w.ToArray();
        }
    }
}