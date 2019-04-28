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
            get
            {
                if (_root != null)
                    return _root.Count;

                return 0;
            }
        }

        private RedBlackNode<TKey, TValue> FindNode(TKey key)
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
            if (_root == null)
            {
                _root = new RedBlackNode<TKey, TValue>(key, value);
                _inspector.FixAfterInsert(_root, null, null);
                return;
            }

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

            if (cur.HasNoChildren)
            {
                if (cur == _root)
                   _root = null;
                else
                {
                    if (parent.Left == cur)
                        parent.Left = null;
                    else
                        parent.Right = null;
                }
            }

            var ind = _comparer.Compare(cur.Key, parent.Key) == 1 ? 1 : 0;

            if (cur.HasOnlyRightChild)
                parent[ind] = cur.Right;
            else if (cur.HasOnlyLeftChild)
                parent[ind] = cur.Left;
            else
            {
                var nextNode = cur.Next;
                RedBlackNode<TKey, TValue>.Swap(cur, nextNode);
                Erase(nextNode.Key);
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
            get
            {
                var res = FindNode(key);
                if (res == null)
                    throw new Exception("Элемент с таким ключом не был найден");

                return res.Value;
            }
            set
            {
                var res = FindNode(key);

                if (res == null)
                    Insert(key, value);
                else
                    res.Value = value;
            }
        }
    }
}
