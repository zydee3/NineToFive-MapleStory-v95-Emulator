using System;

namespace NineToFive.Scripting {
    public class CmdScriptMan : ScriptManager {
        private string _message;

        public CmdScriptMan(Client client, string message) : base(client) {
            _message = message;

            string[] sp = _message.Split(" ");
            Name = sp[0].Substring(1); // remove command prefix
            Args = new string[sp.Length - 1];
            if (sp.Length == 1) return;
            Array.Copy(sp, 1, Args, 0, Args.Length);
        }

        public override void Dispose() {
            base.Dispose();
            _message = null;
            Args = null;
        }

        public string Name { get; }
        public string[] Args { get; private set; }
    }
}