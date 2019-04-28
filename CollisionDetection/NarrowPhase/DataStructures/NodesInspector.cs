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

        private RedBlackNode<TKey, TValue> _root;

        public void FixAfterInsert(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> parent,
            RedBlackNode<TKey, TValue> grandParent)
        {
            if (node == _root)
            {
                node.Red = false;
                return;
            }


            while (RedBlackNode<TKey, TValue>.IsRed(parent))
            {
                var uncle = parent == grandParent.Left ? grandParent.Right : grandParent.Left;

                if (uncle != null && RedBlackNode<TKey, TValue>.IsRed(uncle))
                {
                    parent.Red = false;
                    uncle.Red = false;
                    grandParent.Red = true;
                    node = grandParent;
                }
                else
                {

                }
            }
        }

        public void FixAfterErase(RedBlackNode<TKey, TValue> node, RedBlackNode<TKey, TValue> parent,
            RedBlackNode<TKey, TValue> grandParent)
        {
            if (RedBlackNode<TKey, TValue>.IsRed(node))
                return;
        }
    }
}
