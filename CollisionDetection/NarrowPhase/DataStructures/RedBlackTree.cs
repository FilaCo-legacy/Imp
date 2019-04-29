using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.CollisionDetection.NarrowPhase.DataStructures
{
    internal class RedBlackTree<TKey, TValue>
    {
        private const int LEFT = 0;
        private const int RIGHT = 1;

        private IComparer<TKey> _comparer;

        private RedBlackNode<TKey, TValue> _root;

        public bool Empty => _root == null;

        public TValue this[TKey key]
        {
            get
            {
                var node = FindNode(key);

                if (node == null)
                    throw new Exception("Элемента с таким ключом не было в дереве");

                return node.Value;
            }
            set
            {
                var node = FindNode(key);

                if (node == null)
                    Insert(key, value);
                else
                    node.Value = value;
            }
        }

        private static bool IsRed(RedBlackNode<TKey, TValue> node)
        {
            return node != null && node.Red;
        }

        private RedBlackNode<TKey, TValue> FindNode (TKey key)
        {
            var cur = _root;

            while (cur != null)
            {
                switch (_comparer.Compare(key, cur.Key))
                {
                    case 0:
                        return cur;
                    case -1:
                        cur = cur[LEFT];
                        break;
                    case 1:
                        cur = cur[RIGHT];
                        break;
                }
            }

            return null;
        }

        public RedBlackTree()
        {
            _root = null;
            _comparer = Comparer<TKey>.Default;
        }

        public RedBlackTree(IComparer<TKey> comparer)
        {
            _root = null;
            _comparer = comparer;
        }
             
        public void Insert(TKey key, TValue value)
        {
            // Если дерево пустое
            if (Empty)
                _root = new RedBlackNode<TKey, TValue>(key, value);
            else
            {
                var head = new RedBlackNode<TKey, TValue>();

                var tmpHead = head;
                RedBlackNode<TKey, TValue> grandParent = null;
                RedBlackNode<TKey, TValue> parent = null;

                head[RIGHT] = _root;
                var cur = head[RIGHT];                

                var dir = LEFT;
                var last = dir;

                // Спускаемся вниз по дереву
                while (true)
                {
                    if (cur == null)
                    {
                        // Добавляем вершину вниз
                        cur = new RedBlackNode<TKey, TValue>(key, value);
                        parent[dir] = cur;
                    }
                    else if (IsRed(cur[LEFT]) && IsRed(cur[RIGHT]))
                    {
                        // Смена цветов
                        cur.Red = true;
                        cur[LEFT].Red = false;
                        cur[RIGHT].Red = false;
                    }

                    // Исправление "красного" нарушения
                    if (IsRed(cur) && IsRed(parent))
                    {
                        var sndDir = tmpHead[RIGHT] == grandParent ? 1 : 0;

                        if (cur == parent[last])
                            tmpHead[sndDir] = grandParent.SingleRotate(1 - last);
                        else
                            tmpHead[sndDir] = grandParent.DoubleRotate(1 - last);

                    }

                    if (_comparer.Compare(cur.Key, key) == 0)
                        break;

                    last = dir;
                    dir = _comparer.Compare(key, cur.Key) == -1 ? 1 : 0;

                    if (grandParent != null)
                        tmpHead = grandParent;
                    grandParent = parent;
                    parent = cur;
                    cur = cur[dir];
                }

                _root = head[RIGHT];
            }

            _root.Red = false;
        }

        public void Erase(TKey key)
        {
            if (!Empty)
            {
                var head = new RedBlackNode<TKey, TValue>();

                RedBlackNode<TKey, TValue> grandParent = null;
                RedBlackNode<TKey, TValue> foundNode = null;
                RedBlackNode<TKey, TValue> parent = null;

                var cur = head;
                cur[RIGHT] = _root;

                var dir = RIGHT;

                // Находим и спускаем красные узлы вниз
                while (cur[dir] != null)
                {
                    var last = dir;

                    // Обновляем ссылки
                    grandParent = parent;
                    parent = cur;
                    cur = cur[dir];
                    dir = _comparer.Compare(key, cur.Key) == -1 ? 1 : 0;

                    // Сохраняем найденную вершину
                    if (_comparer.Compare(key, cur.Key) == 0)
                        foundNode = cur;

                    // Толкаем красные вершины вниз
                    if (!IsRed(cur) && !IsRed(cur[dir]))
                    { 
                        if (IsRed(cur[1 - dir]))
                        {
                            parent[last] = cur.SingleRotate(dir);
                            parent = parent[last];
                        }
                        else
                        {
                            var save = parent[1 - last];

                            if (save != null)
                            {
                                if (!IsRed(save[1-last]) && !IsRed(save[1 - last]))
                                {
                                    // Смена цвета
                                    save.Red = false;
                                    save[LEFT].Red = true;
                                    save[RIGHT].Red = true;
                                }
                                else
                                {
                                    var sndDir = grandParent[RIGHT] == parent ? 1 : 0;

                                    if (IsRed(save[last]))
                                        grandParent[sndDir] = parent.DoubleRotate(last);
                                    else if (IsRed(save[1 - last]))
                                        grandParent[sndDir] = parent.SingleRotate(last);

                                    // Красим вершины в корректные цвета
                                    cur.Red = true;
                                    grandParent[sndDir].Red = true;

                                    grandParent[sndDir][LEFT].Red = false;
                                    grandParent[sndDir][RIGHT].Red = false;
                                }
                            }
                        }
                    }
                }
                // Замещаем и удаляем, если найден элемент
                if (foundNode != null)
                {
                    RedBlackNode<TKey, TValue>.Swap(foundNode, cur);

                    parent[cur == parent[RIGHT] ? 1 : 0] = cur[cur[LEFT] == null ? 1 : 0];
                }

                _root = head[RIGHT];

                if (!Empty)
                    _root.Red = false;
            }
        }

        public bool Contains(TKey key)
        {
            return FindNode(key) != null;
        }
        
    }
}
