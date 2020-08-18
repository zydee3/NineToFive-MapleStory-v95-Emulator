using System.Numerics;
using Microsoft.ClearScript.V8;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game {
    public class Skill {
        internal static readonly V8ScriptEngine Engine;
        private int _maxLevel;

        static Skill() {
            Engine = new V8ScriptEngine();
            Engine.Execute(@"
const d = function(n) { return Math.floor(n); }
const u = function(n) { return Math.ceil(n); }");
        }

        public Skill(int id) {
            Id = id;
        }

        public override string ToString() {
            return $"Skill{{Id: {Id}, MasterLevel: {MasterLevel}, MaxLevel: {MaxLevel}, BitMask: {BitMask}}}";
        }

        public TemporaryStat BitMask { get; set; }
        public int Id { get; }
        public int Weapon { get; set; }
        public int MasterLevel { get; set; }

        public int MaxLevel {
            get => _maxLevel;
            set {
                _maxLevel = value;
                Damage = new SkillValue<int>(value);
                PAD = new SkillValue<int>(value);
                PDD = new SkillValue<int>(value);
                MAD = new SkillValue<int>(value);
                MDD = new SkillValue<int>(value);
                Acc = new SkillValue<int>(value);
                Eva = new SkillValue<int>(value);
                Time = new SkillValue<int>(value);
                CoolTime = new SkillValue<int>(value);
                Speed = new SkillValue<int>(value);
                Jump = new SkillValue<int>(value);
                MpCon = new SkillValue<int>(value);
                Lt = new SkillValue<Vector2>(value);
                Rb = new SkillValue<Vector2>(value);
            }
        }

        public SkillValue<int> Damage { get; set; }
        public SkillValue<int> PAD { get; set; }
        public SkillValue<int> PDD { get; set; }
        public SkillValue<int> MAD { get; set; }
        public SkillValue<int> MDD { get; set; }
        public SkillValue<int> Acc { get; set; }
        public SkillValue<int> Eva { get; set; }

        /// <summary>
        /// duration of the buff before expiration
        /// </summary>
        public SkillValue<int> Time { get; set; }

        /// <summary>
        /// time to wait before next skill usage
        /// </summary>
        public SkillValue<int> CoolTime { get; set; }

        public SkillValue<int> Speed { get; set; }
        public SkillValue<int> Jump { get; set; }
        public SkillValue<int> MpCon { get; set; }
        public SkillValue<Vector2> Lt { get; set; }
        public SkillValue<Vector2> Rb { get; set; }
    }

    public class SkillValue<T> {
        private readonly T[] _values;

        public SkillValue(int maxLevel) {
            _values = new T[maxLevel];
        }

        public T this[int skl] {
            get => _values[skl];
            set => _values[skl] = value;
        }

        public void Eval(Skill skill, string expression) {
            for (int skl = 0; skl < skill.MaxLevel; skl++) {
                this[skl] = (T) (object) int.Parse(Skill.Engine.Evaluate($"x={skl + 1}; {expression}").ToString()!);
            }
        }
    }
}