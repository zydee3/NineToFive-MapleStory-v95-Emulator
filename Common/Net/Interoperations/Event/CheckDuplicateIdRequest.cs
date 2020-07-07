using System;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Interopation.Event {
    public static class CheckDuplicateIdRequest {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CheckDuplicateIdRequest));

        public static byte[] OnHandle(Packet r) {
            byte[] result = new byte[1];
            string username = r.ReadString();
            try {
                using DatabaseQuery q = Database.Table("characters");
                using MySqlDataReader reader = q.Select("count(*) as total").Where("username", "=", username).ExecuteReader();
                if (reader.Read()) {
                    if (reader.GetInt32("total") != 0) {
                        // already taken
                        result[0] = 1;
                    }
                } else {
                    // success
                    result[0] = 0;
                }
            } catch (Exception e) {
                Log.Error($"Failed to check availability : {username}", e);
                // unknown error
                result[0] = 3;
            }

            return result;
        }
    }
}