using System;
using System.Collections.Generic;
using Microsoft.ClearScript.V8;

namespace NineToFive.Game {
    public class Skill {
        private static readonly V8ScriptEngine Engine;
        public Dictionary<string, string> Common;

        private int _speed;

        static Skill() {
            Engine = new V8ScriptEngine();
        }

        public Skill(int id) {
            Id = id;
        }

        public override string ToString() {
            return $"Skill{{Id: {Id}, MasterLevel: {MasterLevel}, MaxLevel: {MaxLevel}}}";
        }

        public int Id { get; }
        public int MasterLevel { get; set; }
        public int MaxLevel { get; set; }

        public int Speed(int skl) {
            if (skl < 1) throw new ArgumentOutOfRangeException($"{nameof(skl)} cannot be lower than 0");
            if (Common.TryGetValue("speed", out var e)) {
                return int.Parse(Engine.Evaluate($"x={skl}; {e}").ToString()!);
            }

            return _speed;
        }
    }
}