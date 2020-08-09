using System;
using System.Net;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Util;

namespace NineToFive.Net.Interoperations.Event {
    public static class ClientAuthRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientAuthRequest));

        public static byte[] OnHandle(Packet r) {
            bool updateRequest = r.ReadBool();
            string username = r.ReadString();
            Server.Clients.TryGetValue(username, out Client client);

            if (updateRequest && client != null) {
                using DatabaseQuery query = Database.Table("accounts");
                query.Update(
                    "login_status", (client.LoginStatus = r.ReadByte()),
                    "last_known_ip", r.ReadString()).ExecuteNonQuery();
                return new byte[0]; // result not used
            }

            string password = r.ReadString();
            byte[] machineId = r.ReadBytes(16);

            // todo remove! when login is working as intended
            Log.Info($"login request {username} {password}");

            byte loginResult = ClientTryLogin(ref client, ref username, ref password);

            using Packet w = new Packet();
            w.WriteByte(loginResult);
            if (loginResult == 1) {
                client.MachineId = machineId;
                client.LoginStatus = 1;
                // successful login, encode client data
                client.Encode(client, w);
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
                client.GradeCode = r.GetByte("gm_level");
                client.LoginStatus = r.GetByte("login_status");
                int spwOrdinal = r.GetOrdinal("second_password");
                client.SecondaryPassword = r.IsDBNull(spwOrdinal) ? null : r.GetString(spwOrdinal);
                int lastIpOrdinal = r.GetOrdinal("last_known_ip");
                client.LastKnownIp = r.IsDBNull(lastIpOrdinal) ? null : IPAddress.Parse(r.GetString(lastIpOrdinal));
            } else if (client.LoginStatus != 0) {
                using DatabaseQuery c = Database.Table("accounts");
                using MySqlDataReader r = c.Select().Where("username", "=", username).ExecuteReader();
                // account not found
                if (!r.Read()) return 5;
                if ((client.LoginStatus = r.GetByte("login_status")) != 0) {
                    return 7;
                }
            }

            // incorrect password
            return !password.Equals(client.Password, StringComparison.Ordinal) ? (byte) 4 : (byte) 1;
        }

        public static void RequestClientUpdate(Client client) {
            using Packet w = new Packet();
            w.WriteByte((byte) Interoperation.CheckPasswordRequest);
            w.WriteBool(true);
            w.WriteString(client.Username);
            w.WriteByte(client.LoginStatus); // login_status
            w.WriteString(client.Session.RemoteAddress.ToString());
            Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort, ServerConstants.CentralServer);
        }
    }
}