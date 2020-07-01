using log4net;
using NineToFive.IO;

namespace NineToFive.Interopation.Event {
    public static class ClientAuthRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientAuthRequest));
        private static uint _clientIdIncrement = 1;

        public static byte[] OnHandle(Packet r) {
            string username = r.ReadString();
            string password = r.ReadString();

            CentralServer.Clients.TryGetValue(username, out Client client);
            if (client == null) {
                client = new Client(null, null) {Username = username};
                client.Id = _clientIdIncrement++;
            }

            byte loginResult = client.TryLogin(password);
            using Packet w = new Packet();
            w.WriteByte(loginResult);
            if (loginResult == 1) {
                w.WriteUInt(client.Id);
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