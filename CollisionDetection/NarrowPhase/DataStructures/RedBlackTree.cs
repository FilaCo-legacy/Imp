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

        private RedBlackNode<TKey, TValue> _root;

        public int Count
        {
            get;
        }

        private RedBlackNode<TKey, TValue> FindNode(TKey key, out RedBlackNode<TKey, TValue> parent, 
            out RedBlackNode<TKey, TValue> grandParent)
        {
            var cur = _root;
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

            var ind = _comparer.Compare(cur.Key, parent.Key) == 1 ? 1 : 0;

            if (cur.Left == null && cur.Right == null)
            {
                parent[ind] = null;
            }
            else if (cur.Left == null && cur.Right != null)
            {
                parent[ind] = cur.Right;
            }
            else if (cur.Left != null && cur.Right == null)
            {
                parent[ind] = cur.Left;
            }
            else
            {
                var nextNode = cur.Next;
                RedBlackNode<TKey, TValue>.Swap(cur, nextNode);
                this.Erase(nextNode.Key);
            }

            _inspector.FixAfterErase(cur, parent, grandParent);
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
