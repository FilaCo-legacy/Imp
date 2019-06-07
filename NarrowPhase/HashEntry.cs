using System;
using System.Collections;
using System.Collections.Generic;

namespace ImpLite.NarrowPhase
{
    internal class HashEntry <T> : IEnumerable<T> where  T:IEquatable<T>
    {
        private HashEntry<T> _next;
        public T Value { get; }

        public HashEntry(T value)
        {
            Value = value;
            _next = null;
        }

        public void Add(T value)
        {
            var cur = this;

            while (cur._next != null)
                cur = cur._next;
            
            cur._next = new HashEntry<T>(value);
        }

        public bool Contains(T value)
        {
            var cur = this;

            while (cur != null && !cur.Value.Equals(value))
                cur = cur._next;

            if (cur == null)
                return false;

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var cur = this;

            while (cur != null)
            {
                yield return cur.Value;

                cur = cur._next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}