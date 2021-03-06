using System;
using System.Collections.Generic;

namespace Velo.ECS.Enumeration
{
    public ref struct WhereContext<TEntity>
    {
        public TEntity Current { get; private set; }

        private Dictionary<int, TEntity>.ValueCollection.Enumerator _enumerator;
        private Predicate<TEntity> _predicate;

        public WhereContext(Dictionary<int, TEntity>.ValueCollection.Enumerator enumerator,
            Predicate<TEntity> predicate)
        {
            _enumerator = enumerator;
            _predicate = predicate;

            Current = default;
        }

        public WhereContext<TEntity> GetEnumerator() => this;

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var item = _enumerator.Current;

                if (!_predicate(item)) continue;

                Current = item;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
            _predicate = null;
        }
    }
}