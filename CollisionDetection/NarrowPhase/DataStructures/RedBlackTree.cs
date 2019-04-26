using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.CollisionDetection.NarrowPhase.DataStructures
{
    internal class RedBlackTree<TKey, TValue>
    {
        private NodesInspector<TKey, TValue> _inspector;

        private IComparer<TKey> _comparer;

        internal RedBlackNode<TKey, TValue> Root { get; private set; } 
        
        private RedBlackNode<TKey, TValue> FindNode(TKey key, out RedBlackNode<TKey, TValue> parent, 
            out RedBlackNode<TKey, TValue> grandParent)
        {
            var cur = Root;
            parent = null;
            grandParent = null;

            if (cur == null)
                return null;            

            while (cur != null)
            {
                grandParent = parent;
                parent = cur;

                switch (_comparer.Compare(key, cur.Key))
                {
                    case 0:
                        return cur;
                    case 1:
                        cur = cur.Right;
                        break;
                    case -1:
                        cur = cur.Left;
                        break;
                }
            }

            return null;
        }

        private RedBlackNode<TKey, TValue> FindNode(TKey key)
        {
            RedBlackNode<TKey, TValue> parent, grandParent;

            var cur = FindNode(key, out parent, out grandParent);

            return cur;
        }

        public int Count { get; }

        public void Insert(TKey key, TValue value)
        {
            RedBlackNode<TKey, TValue> parent, grandParent;
            var cur = FindNode(key, out parent, out grandParent);

            if (cur != null)
                return;

            cur = new RedBlackNode<TKey, TValue>(key, value);
            if (_comparer.Compare(key, parent.Key) == -1)
                parent.Left = cur;
            else
                parent.Right = cur;

            _inspector.FixAfterInsert(cur, parent, grandParent);
        }

        public void Erase (TKey key)
        {
            RedBlackNode<TKey, TValue> parent, grandParent;
            var cur = FindNode(key, out parent, out grandParent);

            if (cur == null)
                return;
        }

        public bool Contains(TKey key)
        {
            var cur = FindNode(key);

            return cur != null;
        }

        public TValue this[TKey key]
        {
            get => FindNode(key).Value;
            set => FindNode(key).Value = value;
        }
    }
}
