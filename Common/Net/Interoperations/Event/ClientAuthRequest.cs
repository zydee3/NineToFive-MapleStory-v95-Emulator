using System;
using System.Net;
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
            byte[] machineId = r.ReadBytes(16);

            Server.Clients.TryGetValue(username, out Client client);
            byte loginResult = ClientTryLogin(ref client, ref password);

            using Packet w = new Packet();
            w.WriteByte(loginResult);
            if (loginResult == 1) {
                // successful login, encode client data
                client.Encode(client, w);
                Log.Info($"{client.Username} has logged-in");
                Server.Clients.TryAdd(username, client);
            }

            return w.ToArray();
        }

        private static byte ClientTryLogin(ref Client client, ref string password) {
            if (client == null) {
                using DatabaseQuery c = Database.Table("accounts");
                using MySqlDataReader r = c.Select().Where("username", "=", client.Username).ExecuteReader();
                // account not found
                if (!r.Read()) return 5;
                client = new Client(null, null) {
                    Id = r.GetUInt32("id"),
                    Username = r.GetString("username"),
                    Password = r.GetString("password"),
                    Gender = r.GetByte("gender"),
                    SecondaryPassword = r.GetString("second_password"),
                    LastKnownIp = IPAddress.Parse(r.GetString("last_known_ip")),
                };
            }

            // incorrect password
            return !password.Equals(client.Password, StringComparison.Ordinal) ? (byte) 4 : (byte) 1;
        }
    }
}