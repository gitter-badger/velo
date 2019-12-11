using System;
using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct GroupEnumerator<TKey>
        {
            public readonly LocalGroup Current => _current;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private EqualityComparer<TKey> _comparer;
            private LocalVector<Row<TKey, T>> _values;
            private LocalVector<TKey> _uniqueKeys;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            private LocalGroup _current;
            private int _position;

            internal GroupEnumerator(in LocalVector<T> vector, Func<T, TKey> keySelector, EqualityComparer<TKey> comparer)
            {
                var values = new LocalVector<Row<TKey, T>>(vector.Length);
                var uniqueKeys = new LocalVector<TKey>();

                for (var i = 0; i < vector.Length; i++)
                {
                    var element = vector.Get(i);
                    var key = keySelector(element);

                    values.Add(new Row<TKey, T>(key, element));

                    if (!uniqueKeys.Contains(key, comparer))
                    {
                        uniqueKeys.Add(key);
                    }
                }

                _comparer = comparer;
                _current = default;
                _position = 0;
                _values = values;
                _uniqueKeys = uniqueKeys;
            }

            public readonly GroupEnumerator<TKey> GetEnumerator() => this;

            public bool MoveNext()
            {
                if (_position >= _uniqueKeys.Length) return false;

                var currentKey = _uniqueKeys.Get(_position);

                var groupVector = new LocalVector<T>();
                for (var i = 0; i < _values.Length; i++)
                {
                    var element = _values.Get(i);
                    if (_comparer.Equals(currentKey, element.Key))
                    {
                        groupVector.Add(element.Value);
                    }
                }

                _current = new LocalGroup(currentKey, groupVector);
                _position++;

                return true;
            }

            public LocalVector<TResult> Select<TResult>(Selector<TResult> selector)
            {
                var result = new LocalVector<TResult>(_uniqueKeys._length);
                while (MoveNext())
                {
                    var element = selector(_current);
                    result.Add(element);
                }

                return result;
            }

            public int Sum(Selector<int> selector)
            {
                var sum = 0;
                
                while (MoveNext())
                {
                    sum += selector(_current);
                }

                return sum;
            }
            
            public readonly ref struct LocalGroup
            {
                public readonly TKey Key;

                public readonly LocalVector<T> Values;

                internal LocalGroup(TKey key, LocalVector<T> values)
                {
                    Key = key;
                    Values = values;
                }

                public T First() => Values[0];
                
                public Enumerator GetEnumerator() => Values.GetEnumerator();

                public SelectEnumerator<TValue> Select<TValue>(Func<T, TValue> selector)
                {
                    return Values.Select(selector);
                }

                public SelectEnumerator<TValue, TArg> Select<TValue, TArg>(Func<T, TArg, TValue> selector, TArg arg)
                {
                    return Values.Select(selector, arg);
                }

                public int Sum(Func<T, int> selector)
                {
                    return Values.Sum(selector);
                }
                
                public WhereEnumerator Where(Predicate<T> predicate)
                {
                    return Values.Where(predicate);
                }

                public WhereEnumerator<TArg> Where<TArg>(Func<T, TArg, bool> predicate, TArg arg)
                {
                    return Values.Where(predicate, arg);
                }
            }

            public delegate TResult Selector<out TResult>(LocalGroup group);
        }
    }
}