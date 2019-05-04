using System;

namespace PhysEngine.CollisionDetection.NarrowPhase.DataStructures
{
    /// <summary>
    /// Вспомогательный класс, представляющий собой вершину <see cref="RedBlackTree{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class RedBlackNode<TKey, TValue>
    {
        public const int LEFT = 0;
        public const int RIGHT = 1;

        private readonly RedBlackNode<TKey, TValue>[] _link;

        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public bool Red { get; set; }        

        public RedBlackNode<TKey, TValue> this[int dir]
        {
            get
            {
                if (dir != LEFT && dir != RIGHT)
                    throw new Exception("Некорректный индекс ребёнка");
                return _link[dir];
            }
            set
            {
                if (dir != LEFT && dir != RIGHT)
                    throw new Exception("Некорректный индекс ребёнка");
                _link[dir] = value;
            }
        }

        public int Count
        {
            get
            {
                var sum = 1;

                if (this[LEFT] != null)
                    sum += this[LEFT].Count;

                if (this[RIGHT] != null)
                    sum += this[RIGHT].Count;

                return sum;
            }
        }

        public RedBlackNode()
        {
            Key = default(TKey);
            Value = default(TValue);
            Red = true;
            _link = new RedBlackNode<TKey, TValue>[2];
        }

        public RedBlackNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Red = true;
            _link = new RedBlackNode<TKey, TValue>[2];
        }

        public RedBlackNode<TKey, TValue> SingleRotate(int dir)
        {
            if (dir != LEFT && dir != RIGHT)
                throw new Exception("Некорректное направление поворота");

            var save = this[1 - dir];
            this[1 - dir] = save[dir];
            save[dir] = this;

            Red = true;
            save.Red = false;

            return save;
        }

        public RedBlackNode<TKey, TValue> DoubleRotate(int dir)
        {
            this[1 - dir] = this[1 - dir].SingleRotate(1 - dir);
            return SingleRotate(dir);
        }

        public static void Swap(RedBlackNode<TKey, TValue> nodeA, RedBlackNode<TKey, TValue> nodeB)
        {
            var tmpKey = nodeA.Key;
            nodeA.Key = nodeB.Key;
            nodeB.Key = tmpKey;

            var tmpValue = nodeA.Value;
            nodeA.Value = nodeB.Value;
            nodeB.Value = tmpValue;
        }        

        public static bool IsRed(RedBlackNode<TKey, TValue> node)
        {
            return node != null && node.Red;
        }
    }
}
