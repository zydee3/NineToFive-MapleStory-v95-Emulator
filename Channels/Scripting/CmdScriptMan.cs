using System;
using log4net;

namespace NineToFive.Scripting {
    public class CmdScriptMan : ScriptManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CmdScriptMan));
        private string _message;

        public CmdScriptMan(Client client, string message) : base(client) {
            _message = message;

            string[] sp = _message.Split(" ");
            Prefix = sp[0].Substring(0, 1);
            Name = sp[0].Substring(Prefix.Length); // remove command prefix
            Args = new string[sp.Length - 1];
            if (sp.Length == 1) return;
            Array.Copy(sp, 1, Args, 0, Args.Length);
        }

        public override void Dispose() {
            base.Dispose();
            _message = null;
            Args = null;
        }

        public string Prefix { get; set; }
        public string Name { get; }
        public string[] Args { get; private set; }
        public int ArgAsInt(int idx) => int.Parse(Args[idx]);

        public override void Print(string message) {
            Log.Info($"[{Name}] : {message}");
        }
    }
}