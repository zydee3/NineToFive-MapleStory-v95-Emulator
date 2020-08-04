using System;
using NineToFive.Game.Entity;

namespace NineToFive.Scripting {
    public class ScriptManager : IDisposable {
        public ScriptManager(Client client) {
            Client = client;
        }

        public virtual void Dispose() {
            Client = null;
        }

        public Client Client { get; private set; }
        public User User => Client.User;

        public virtual void Print(string message) { }
    }
}