using System;
using System.Net;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Interopation.Event {
    public static class ClientAuthRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientAuthRequest));

        public static byte[] OnHandle(Packet r) {
            string username = r.ReadString();
            string password = r.ReadString();
            byte[] machineId = r.ReadBytes(16);

            Server.Clients.TryGetValue(username, out Client client);
            byte loginResult = ClientTryLogin(ref client, ref username, ref password);

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

        private static byte ClientTryLogin(ref Client client, ref string username, ref string password) {
            if (client == null) {
                using DatabaseQuery c = Database.Table("accounts");
                using MySqlDataReader r = c.Select().Where("username", "=", username).ExecuteReader();
                // account not found
                if (!r.Read()) return 5;
                client = new Client();
                client.Id = r.GetUInt32("account_id");
                client.Username = r.GetString("username");
                client.Password = r.GetString("password");
                client.Gender = r.GetByte("gender");
                // im so angry why does DBNull exist i want to smash my keyboard i can't believe GetString can't simply return null...
                // someone really thought DBNull had to exist to make the lives of everyone inconvenient by one extra getter method
                int spwOrdinal = r.GetOrdinal("second_password");
                client.SecondaryPassword = r.IsDBNull(spwOrdinal) ? null : r.GetString(spwOrdinal);
                int lastIpOrdinal = r.GetOrdinal("last_known_ip");
                client.LastKnownIp = r.IsDBNull(lastIpOrdinal) ? null : IPAddress.Parse(r.GetString(lastIpOrdinal));
            }

            // incorrect password
            return !password.Equals(client.Password, StringComparison.Ordinal) ? (byte) 4 : (byte) 1;
        }
    }
}