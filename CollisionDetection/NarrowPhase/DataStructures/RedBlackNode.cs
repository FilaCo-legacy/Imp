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
        internal static bool IsRed(RedBlackNode<TKey, TValue> node)
        {
            return node != null && node.Red;
        }

        internal TKey Key { get; set; }

        internal TValue Value { get; set; }

        internal bool Red { get; set; }

        internal RedBlackNode<TKey, TValue> Left { get; set; }

        public RedBlackNode<TKey, TValue> Right { get; set; }

        /// <summary>
        /// Представляет доступ к детям по индексу
        /// </summary>
        /// <param name="dir">Индекс ребёнка</param>
        /// <returns>Если dir == 0, то возвращает левого ребёнка, иначе если dir == 1, то правого</returns>
        public RedBlackNode<TKey, TValue> this[int dir]
        {
            get
            {
                if (dir == 0)
                    return Left;
                else if (dir == 1)
                    return Right;

                throw new Exception("Некорректный индекс ребёнка подан на вход");
            }
            set
            {
                if (dir == 0)
                    Left = value;
                else if (dir == 1)
                    Right = value;

                throw new Exception("Некорректный индекс ребёнка подан на вход");
            }
        }

        /// <summary>
        /// Одинарный поворот вершины дерева в заданном направлении
        /// </summary>
        /// <param name="dir">Направление вращения</param>
        /// <returns>Возвращает вершину, оказавшуюся на заданной позиции</returns>
        public RedBlackNode<TKey, TValue> SingleRotate(int dir)
        {
            var save = this[1 - dir];

            this[1 - dir] = save[dir];
            save[dir] = this;

            this.Red = true;
            save.Red = false;

            return save;
        }

        /// <summary>
        /// Двойной поворот вершины дерева в заданном направлении
        /// </summary>
        /// <param name="dir">Направление вращения</param>
        /// <returns></returns>
        public RedBlackNode<TKey, TValue> DoubleRotate(int dir)
        {
            this[1 - dir] = this[1 - dir].SingleRotate(1 - dir);

            return this.SingleRotate(dir);
        }

        public RedBlackNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Red = true;
            Left = null;
            Right = null;
        }
    }
}
