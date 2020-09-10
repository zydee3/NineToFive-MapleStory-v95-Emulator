using System.Collections;
using System.Collections.Generic;
using Microsoft.ClearScript.V8;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;

namespace NineToFive.Game {
    public class Skill {
        internal static readonly V8ScriptEngine Engine;
        private int _maxLevel;

        static Skill() {
            Engine = new V8ScriptEngine();
            Engine.Execute(@"
                const d = function(n) { return Math.floor(n); }
                const u = function(n) { return Math.ceil(n); }"
            );
        }

        public Skill(int id) {
            Id = id;

            CTS = new Dictionary<SecondaryStat, SkillValue>(7);
        }

        public override string ToString() {
            return $"Skill{{Id: {Id}, MasterLevel: {MasterLevel}, MaxLevel: {MaxLevel}}}";
        }

        public void EncodeBitmask(Packet w) {
            var array = new int[4];
            var bits = new BitArray(128);
            foreach (var stat in CTS.Keys) {
                bits[(int) stat] = true;
            }
            bits.CopyTo(array, 0);
            for (var i = array.Length - 1; i >= 0; i--) {
                w.WriteInt(array[i]);
            }
        }

        public Dictionary<SecondaryStat, SkillValue> CTS { get; set; }
        public int Id { get; }
        public int Weapon { get; set; }
        public int MasterLevel { get; set; }

        /// <summary>
        /// 1    for weapon mastery
        /// 2    for booster
        /// 3    for final attack
        /// </summary>
        public byte SkillType { get; set; }

        public bool IsActive { get; set; }

        public int MaxLevel {
            get => _maxLevel;
            set {
                _maxLevel = value;
                Damage = new SkillValue(value);
                MobCount = new SkillValue(value);
                Range = new SkillValue(value);
                AttackCount = new SkillValue(value);
                Time = new SkillValue(value);
                CoolTime = new SkillValue(value);
                MpCon = new SkillValue(value);
                X = new SkillValue(value);
                Y = new SkillValue(value);
                Lt = new SkillValue(value);
                Rb = new SkillValue(value);
            }
        }

        public SkillValue Damage { get; set; }
        public SkillValue MobCount { get; set; }
        public SkillValue Range { get; set; }
        public SkillValue AttackCount { get; set; }


        /// <summary>
        /// duration of the buff before expiration
        /// </summary>
        public SkillValue Time { get; set; }

        /// <summary>
        /// time to wait before next skill usage
        /// </summary>
        public SkillValue CoolTime { get; set; }

        public SkillValue MpCon { get; set; }
        public SkillValue X { get; set; }
        public SkillValue Y { get; set; }
        public SkillValue Lt { get; set; }
        public SkillValue Rb { get; set; }
    }

    public class SkillValue : IEnumerator {
        private int _position;
        private readonly object[] _values;

        public SkillValue(int maxLevel, object value = null) {
            _values = new object[maxLevel];
            if (value == null) return;

            for (int i = 0; i < maxLevel; i++) {
                _values[i] = value;
            }
        }

        public object this[int skl] {
            get => _values[skl];
            set => _values[skl] = value;
        }

        public void Eval(Skill skill, string expression) {
            for (int skl = 0; skl < skill.MaxLevel; skl++) {
                this[skl] = int.Parse(Skill.Engine.Evaluate($"x={skl + 1}; {expression}").ToString()!);
            }
        }

        public bool MoveNext() {
            _position++;
            return _position < _values.Length;
        }

        public void Reset() {
            _position = 0;
        }

        public object Current => _values[_position];
    }
}