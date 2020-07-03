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

            CentralServer.Clients.TryGetValue(username, out Client client);
            client ??= new Client(null, null) {
                Username = username
            };

            byte loginResult = ClientTryLogin(ref client, ref password);

            using Packet w = new Packet();
            w.WriteByte(loginResult);
            if (loginResult == 1) {
                // successful login, encode client data
                client.Encode(client, w);
                Log.Info($"{client.Username} has logged-in");
                CentralServer.Clients.TryAdd(username, client);
            }

            return w.ToArray();
        }

        private static byte ClientTryLogin(ref Client client, ref string password) {
            using DatabaseQuery c = Database.Table("accounts");
            using MySqlDataReader r = c.Select().Where("username", "=", client.Username).ExecuteReader();
            // account not found
            if (!r.Read()) return 5;
            string sPassword = r.GetString("password");
            // incorrect password
            if (!sPassword.Equals(password, StringComparison.Ordinal)) {
                Log.Info($"Account '{client.Username}' does not exist");
                return 4;
            }

            client.Id = r.GetUInt32("id");
            client.Username = r.GetString("username");
            client.Gender = r.GetByte("gender");
            client.SecondaryPassword = r.GetString("second_password");
            client.LastKnownIp = IPAddress.Parse(r.GetString("last_known_ip"));
            return 1;
        }
    }
}