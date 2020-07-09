using System;
using System.IO;
using System.Threading.Tasks;
using log4net;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace NineToFive.Scripting {
    // https://github.com/microsoft/ClearScript/issues/182 thank you!
    public static class TaskScripting {
        private delegate void Executor(dynamic resolve, dynamic reject);

        public static object ToPromise(this Task task) {
            var ctor = (ScriptObject) ScriptEngine.Current.Script.Promise;
            return ctor.Invoke(true, new Executor((resolve, reject) => {
                task.ContinueWith(t => {
                    if (t.IsCompleted) {
                        resolve();
                    } else {
                        reject(t.Exception);
                    }
                });
            }));
        }
    }

    public static class Scriptable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Scriptable));
        private const string Root = "Resources/Scripts";

        static Scriptable() {
            Directory.CreateDirectory($"{Root}/Commands");
            Directory.CreateDirectory($"{Root}/Npc");
        }

        public static async Task<object> RunScriptAsync(string path, Action<V8ScriptEngine> consumer = null) {
            path = $"{Root}/{path}";
            if (!File.Exists(path)) {
                throw new FileNotFoundException(path);
            }

            using var engine = new V8ScriptEngine();
            engine.AddHostType(typeof(Task));
            engine.AddHostType(typeof(TaskScripting));
            engine.AddHostType(typeof(Console));
            engine.AddHostType(typeof(Convert));
            consumer?.Invoke(engine);

            string text = await File.ReadAllTextAsync(path);
            engine.Execute(text);

            // await engine.Script.run().ToTask();
            return await engine.Evaluate("run();").ToTask();
        }
    }
}