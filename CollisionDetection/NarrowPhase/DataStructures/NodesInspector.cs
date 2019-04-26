using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.CollisionDetection.NarrowPhase.DataStructures
{
    internal class NodesInspector<TKey, TValue>
    {
        private const int LEFT = 0;
        private const int RIGHT = 1;

        private RedBlackTree<TKey, TValue> _tree;

        internal void FixAfterInsert(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> nodeParent,
            RedBlackNode<TKey, TValue> nodeGrandParent)
        {

        }

        internal void FixAfterErase(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> nodeParent,
            RedBlackNode<TKey, TValue> nodeGrandParent)
        {

        }
    }
}
