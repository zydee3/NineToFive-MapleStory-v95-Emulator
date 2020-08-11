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
        private static readonly string Root;

        static Scriptable() {
            Root = $"{Directory.GetCurrentDirectory()}/Resources/Scripts";
            Directory.CreateDirectory($"{Root}/Commands");
            Directory.CreateDirectory($"{Root}/Npc");
        }

        public static async Task<object> RunScriptAsync(V8ScriptEngine engine, string function = "run();") {
            return await engine.Evaluate(function).ToTask();
        }

        public static async Task<V8ScriptEngine> GetEngine(string path, ScriptManager manager) {
            path = $"{Root}/{path}";
            if (!File.Exists(path)) {
                throw new FileNotFoundException(path);
            }

            V8ScriptEngine engine = new V8ScriptEngine {
                AllowReflection = true
            };
            // required for async execution, functions need to have the 'async' modifer
            engine.AddHostType(typeof(Task));
            engine.AddHostType(typeof(TaskScripting));
            // typically for Console.WriteLine debugging
            engine.AddHostType(typeof(Console));
            // imports and reflection
            engine.AddHostObject("Host", new ExtendedHostFunctions());
            // script context
            engine.AddHostObject("ctx", manager);

            string text = await File.ReadAllTextAsync(path);
            engine.Execute(text);

            return engine;
        }
    }
}