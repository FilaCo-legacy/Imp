using System;
using System.Linq;

namespace ImpLite.NarrowPhase
{
    internal class HashTable <T> where T : IEquatable<T>
    {
        private readonly HashEntry<T>[] _table;

        public const int MaxTableSize = (int) 5e6 + 11;

        public HashTable(int size = MaxTableSize)
        {
            _table = new HashEntry<T>[size];

            Clear();
        }

        private int GetHashCode(T value)
        {
            return value.GetHashCode() % _table.Length;
        }
        
        public void Add(T value)
        {
            var hash = GetHashCode(value);
            
            if (_table[hash] == null)
                _table[hash] = new HashEntry<T>(value);
            else if (!_table[hash].Contains(value))
                _table[hash].Add(value);
        }

        public bool FindByHash(int hash, out T value)
        {
            value = default;
            
            hash %= _table.Length;

            if (_table[hash] == null)
                return false;

            var values = _table[hash].ToArray();
            
            value = values[0];

            return values.Length == 1;
        }

        public bool Contains(T value)
        {
            var hash = GetHashCode(value);

            return _table != null && _table[hash].Contains(value);
        }

        public void Clear()
        {
            for (var i = 0; i < _table.Length; ++i)
                _table[i] = null;   
        }
    }
}