using System;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Interopation.Event {
    public static class ClientAuthRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientAuthRequest));
        private static uint _clientIdIncrement = 1;

        public static byte[] OnHandle(Packet r) {
            string username = r.ReadString();
            string password = r.ReadString();

            CentralServer.Clients.TryGetValue(username, out Client client);
            if (client == null) {
                client = new Client(null, null) {Id = _clientIdIncrement++};
            }

            byte loginResult = ClientTryLogin(username, password);
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
        
        private static byte ClientTryLogin(string username, string password) {
            byte result = 4;
            using DatabaseQuery c = Database.Table("accounts");
            using MySqlDataReader r = c.Select("*").Where("username", "=", username).ExecuteReader();
            if (r.Read()) {
                string sPassword = r.GetString("password");
                if (sPassword.Equals(password, StringComparison.Ordinal)) {
                    result = 1;
                }
            }

            return result;
        }
    }
}