using System.Collections.Generic;

namespace ImpLite.BroadPhase
{
    /// <summary>
    /// Data structure "Quadrant tree": https://en.wikipedia.org/wiki/Quadtree
    /// </summary>
    internal class QuadTree<T> : IBoxTree<T> where T : IBoxable
    {
        /// <summary>
        /// Maximum number of objects that might be located in one node
        /// </summary>
        public const int MaxObjects = 10;

        /// <summary>
        /// Maximum levels that can be created
        /// </summary>
        public const int MaxLevels = 5;

        /// <summary>
        /// Level of this <see cref="QuadTree{T}"/> node.
        /// 0 - the highest level
        /// </summary>
        private readonly int _level;

        /// <summary>
        /// Objects that are located in this <see cref="QuadTree{T}"/> node
        /// </summary>
        private readonly List<T> _objects;

        /// <summary>
        /// Bounds of this <see cref="QuadTree{T}"/> node
        /// </summary>
        private readonly Box _bounds;

        /// <summary>
        /// Children nodes
        /// </summary>
        private readonly QuadTree<T>[] _nodes;

        /// <summary>
        /// Splits current <see cref="QuadTree{T}"/> node and creates children of it
        /// </summary>
        private void Split()
        {
            var subWidth = _bounds.Width * 0.5f;
            var subHeight = _bounds.Height * 0.5f;

            var x = _bounds.LeftUpper.X;
            var y = _bounds.LeftUpper.Y;

            _nodes[0] = new QuadTree<T>(_level + 1, new Box(x + subWidth, y, subWidth, subHeight));
            _nodes[1] = new QuadTree<T>(_level + 1, new Box(x, y, subWidth, subHeight));
            _nodes[2] = new QuadTree<T>(_level + 1, new Box(x, y + subWidth, subWidth, subHeight));
            _nodes[3] = new QuadTree<T>(_level + 1, new Box(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private bool IsInTopQuadrants(Box rectangle)
        {
            return rectangle.LeftUpper.Y < _bounds.Center.Y &&
                   (rectangle.LeftUpper.Y + rectangle.Height < _bounds.Center.Y);
        }

        private bool IsInBottomQuadrants(Box rectangle)
        {
            return rectangle.LeftUpper.Y > _bounds.Center.Y;
        }

        private bool IsInLeftQuadrants(Box rectangle)
        {
            return rectangle.LeftUpper.X < _bounds.Center.X &&
                   (rectangle.LeftUpper.X + rectangle.Width < _bounds.Center.X);
        }

        private bool IsInRightQuadrants(Box rectangle)
        {
            return rectangle.LeftUpper.X > _bounds.Center.X;
        }

        /// <summary>
        /// Gets index of a child node, where the <see cref="Box"/> is located
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>Returns "-1", if the <see cref="Box"/> is located at least in two quads,
        /// else returns the quad index</returns>
        private int GetIndex(Box rectangle)
        {
            var index = -1;

            if (IsInRightQuadrants(rectangle))
            {
                if (IsInTopQuadrants(rectangle))
                    index = 0;
                else if (IsInBottomQuadrants(rectangle)) index = 3;
            }
            else if (IsInLeftQuadrants(rectangle))
            {
                if (IsInTopQuadrants(rectangle))
                    index = 1;
                else if (IsInBottomQuadrants(rectangle)) index = 2;
            }

            return index;
        }

        private bool TryInsertChildren(T obj)
        {
            if (_nodes[0] is null) return false;
            
            var index = GetIndex(obj.GetBox);

            if (index == -1) return false;
            
            _nodes[index].Insert(obj);
            return true;
        }

        public QuadTree(int level, Box bounds)
        {
            _level = level;
            _bounds = bounds;
            _nodes = new QuadTree<T>[4];
            _objects = new List<T>();
        }

        public void Clear()
        {
            _objects.Clear();

            for (var i = 0; i < _nodes.Length; ++i)
            {
                if (_nodes[i] is null) continue;
                
                _nodes[i].Clear();
                _nodes[i] = null;
            }
        }

        /// <summary>
        /// Puts a new object in the <see cref="QuadTree{T}"/>
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(T obj)
        {
            if (TryInsertChildren(obj)) return;

            _objects.Add(obj);

            if (_objects.Count <= MaxObjects || _level >= MaxLevels) return;
            
            if (!(_nodes[0] is null)) Split();

            var i = 0;
            while (i < _objects.Count)
            {
                if (TryInsertChildren(_objects[i]))
                    _objects.RemoveAt(i);
                else
                    ++i;
            }
        }

        /// <summary>
        /// Finds objects that are possible to collide with given
        /// </summary>
        /// <param name="targetObj">Target object</param>
        /// <param name="neighbours">List of neighbours</param>
        public void Retrieve(List<T> neighbours, T targetObj)
        {
            var index = GetIndex(targetObj.GetBox);

            if (index != -1 && !(_nodes[0] is null)) _nodes[index].Retrieve(neighbours, targetObj);

            neighbours.AddRange(_objects);
        }
    }
}