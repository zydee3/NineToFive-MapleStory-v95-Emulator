using System;
using System.Collections.Generic;

namespace NineToFive {
    /// <summary>
    ///  contains cached data to reduce overhead of querying the database 
    /// </summary>
    public static class CentralServer {
        public static readonly Dictionary<string, Client> Clients = new Dictionary<string, Client>(StringComparer.OrdinalIgnoreCase);
    }
}