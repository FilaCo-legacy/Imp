using System.Collections.Generic;

namespace PhysEngine.Collision.BroadPhase
{
    /// <summary>
    /// Структура данных дерево квандрантов
    /// </summary>
    public class QuadTree
    {
        public const int MAX_OBJECTS = 10;
        public const int MAX_LEVELS = 5;

        /// <summary>
        /// Уровень вершины. 0 - самый верхний уровень
        /// </summary>
        private int _level;

        /// <summary>
        /// Объекты, представленные в виде AABB, находящиеся в данной вершине
        /// </summary>
        private List<AABB> _objects;

        /// <summary>
        /// Границы данной вершины на внешней карте
        /// </summary>
        private AABB _bounds;

        /// <summary>
        /// Дочерние узлы дерева
        /// </summary>
        private QuadTree[] _nodes;

        /// <summary>
        /// Разделение узла дерева и определение его четырёх потомков
        /// </summary>
        private void Split()
        {
            var subWidth = _bounds.Width * 0.5f;
            var subHeight = _bounds.Height * 0.5f;

            var x = _bounds.LeftUpper.X;
            var y = _bounds.LeftUpper.Y;

            _nodes[0] = new QuadTree(_level + 1, new AABB(x + subWidth, y, subWidth, subHeight));
            _nodes[1] = new QuadTree(_level + 1, new AABB(x, y, subWidth, subHeight));
            _nodes[2] = new QuadTree(_level + 1, new AABB(x, y + subWidth, subWidth, subHeight));
            _nodes[3] = new QuadTree(_level + 1, new AABB(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private bool IsInTopQuadrants(AABB rectangle)
        {
            return (rectangle.LeftUpper.Y < _bounds.Center.Y) && (rectangle.LeftUpper.Y + rectangle.Height < _bounds.Center.Y);
        }

        private bool IsInBottomQuadrants(AABB rectangle)
        {
            return rectangle.LeftUpper.Y > _bounds.Center.Y;
        }

        private bool IsInLeftQuadrants(AABB rectangle)
        {
            return (rectangle.LeftUpper.X < _bounds.Center.X) && (rectangle.LeftUpper.X + rectangle.Width < _bounds.Center.X);
        }

        private bool IsInRightQuadrants(AABB rectangle)
        {
            return rectangle.LeftUpper.X > _bounds.Center.X;
        }

        /// <summary>
        /// Получает индекс дочернего узла, в котором полностью находится указанный объект
        /// </summary>
        /// <param name="rectangle">Объект, месторасположение которого выясняется</param>
        /// <returns>Возвращает -1, если объект лежит хотя бы на двух квадрантах, иначе возвращает индекс четверти</returns>
        private int GetIndex(AABB rectangle)
        {
            var index = -1;

            if (IsInRightQuadrants(rectangle))
            {
                if (IsInTopQuadrants(rectangle))
                    index = 0;
                else if (IsInBottomQuadrants(rectangle))
                    index = 3;
            }
            else if (IsInLeftQuadrants(rectangle))
            {
                if (IsInTopQuadrants(rectangle))
                    index = 1;
                else if (IsInBottomQuadrants(rectangle))
                    index = 2;
            }
            return index;
        }

        private bool TryInsertChildren(AABB rectangle)
        {
            if (!(_nodes[0] is null))
            {
                var index = GetIndex(rectangle);

                if (index != -1)
                {
                    _nodes[index].Insert(rectangle);
                    return true;
                }
            }
            return false;
        }

        public QuadTree(int level, AABB bounds)
        {
            _level = level;
            _bounds = bounds;
            _nodes = new QuadTree[4];
            _objects = new List<AABB>();
        }

        public void Clear()
        {
            _objects.Clear();

            for (var i = 0; i < _nodes.Length; ++i)
            {
                if (!(_nodes[i] is null))
                {
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        /// <summary>
        /// Помещает новый объект в дерево
        /// </summary>
        /// <param name="rectangle"></param>
        public void Insert(AABB rectangle)
        {
            if (TryInsertChildren(rectangle))
            {
                return;
            }

            _objects.Add(rectangle);

            if (_objects.Count > MAX_OBJECTS && _level < MAX_LEVELS)
            {
                if (!(_nodes[0] is null))
                {
                    Split();
                }

                var i = 0;
                while (i < _objects.Count)
                {
                    if (TryInsertChildren(_objects[i]))
                    {
                        _objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Определяет, какие объекты возможно пересекаются с данным
        /// </summary>
        /// <param name="rectangle">Проверяемый объект</param>
        /// <param name="candidates">Список кандидатов на пересечение</param>
        public void Retrieve(List<AABB> candidates, AABB rectangle)
        {
            var index = GetIndex(rectangle);
            
            if (index != -1 && !(_nodes[0] is null))
            {
                _nodes[index].Retrieve(candidates, rectangle);
            }

            candidates.AddRange(_objects);

        }
    }
}
