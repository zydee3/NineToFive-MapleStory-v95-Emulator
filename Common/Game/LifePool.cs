using System.Collections.Generic;
using System.Threading;
using NineToFive.Game.Entity;

namespace NineToFive.Game {
    public class LifePool<T> where T : Life {
        private readonly Dictionary<uint, T> _pool = new Dictionary<uint, T>();
        private int _uniqueId = 0;

        public Dictionary<uint, T>.ValueCollection Values => _pool.Values;
        public int Count => _pool.Count;

        public T this[uint poolId] {
            get {
                _pool.TryGetValue(poolId, out T v);
                return v;
            }
        }

        public void AddLife(T t) {
            int uniqueId = Interlocked.Increment(ref _uniqueId);
            t.PoolId = unchecked((uint) uniqueId);
            _pool.Add(t.PoolId, t);
        }

        public bool RemoveLife(Life life) {
            return _pool.Remove(life.PoolId);
        }
    }
}