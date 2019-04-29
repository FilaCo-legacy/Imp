namespace PhysEngine.CollisionDetection.NarrowPhase.DataStructures
{
    /// <summary>
    /// Вспомогательный класс, представляющий собой вершину <see cref="RedBlackTree{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class RedBlackNode<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public bool IsRed { get; set; }

        public RedBlackNode<TKey, TValue> Parent { get; set; }

        public RedBlackNode<TKey, TValue> Left { get; set; }

        public RedBlackNode<TKey, TValue> Right { get; set; }

        public RedBlackNode<TKey, TValue> GrandParent
        {
            get
            {
                if (Parent != null)
                    return Parent.Parent;

                return null;
            }
        }

        public RedBlackNode<TKey, TValue> Brother
        {
            get
            {
                if (Parent == null)
                    return null;

                return this == Parent.Left ? Parent.Right : Parent.Left;
            }
        }

        public RedBlackNode<TKey, TValue> Uncle
        {
            get
            {
                var grandParent = GrandParent;

                if (grandParent == null)
                    return null;

                if (Parent == grandParent.Left)
                    return grandParent.Right;
                else
                    return grandParent.Left;
            }
        }

        public bool HasNoChildren => Left == null && Right == null;

        public bool HasOnlyLeftChild => Left != null && Right == null;

        public bool HasOnlyRightChild => Left == null && Right != null;

        public bool HasBothChildren => Left != null && Right != null;

        public RedBlackNode<TKey, TValue> Pref
        {
            get
            {
                if (Left == null)
                    return Parent;

                return Left.FindMaxDescendant();
            }
        }

        public RedBlackNode<TKey, TValue> Next
        {
            get
            {
                if (Right == null)
                    return Parent;

                return Right.FindMinDescendant();
            }
        }

        private RedBlackNode<TKey, TValue> FindMinDescendant()
        {
            if (Left != null)
                return Left.FindMinDescendant();
            return this;
        }

        private RedBlackNode<TKey, TValue> FindMaxDescendant()
        {
            if (Right != null)
                return Right.FindMaxDescendant();
            return this;
        }

        public RedBlackNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            IsRed = true;
            Left = null;
            Right = null;
        }

        public void LeftRotate()
        {
            var pivot = Right;

            pivot.Parent = Parent;
            if (Parent == null)
            {
                if (Parent.Left == this)
                    Parent.Left = pivot;
                else
                    Parent.Right = pivot;
            }

            Right = pivot.Left;
            if (pivot.Left != null)
                pivot.Left.Parent = this;

            Parent = pivot;
            pivot.Left = this;
        }

        public void RightRotate()
        {
            var pivot = Left;

            pivot.Parent = Parent;
            if (Parent == null)
            {
                if (Parent.Left == this)
                    Parent.Left = pivot;
                else
                    Parent.Right = pivot;
            }

            Left = pivot.Right;
            if (pivot.Right != null)
                pivot.Right.Parent = this;

            Parent = pivot;
            pivot.Right = this;
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
    }
}
