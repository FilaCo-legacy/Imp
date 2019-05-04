using System.Collections.Generic;
using PhysEngine.CollisionDetection.BroadPhase.DataStructures;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    /// <summary>
    /// Класс, отвечающий за "Широкую фазу" разрешения коллизий.
    /// </summary>
    internal class BroadPhaseManager <T> where T: IBoxable
    {
        private QuadTree<T> _quadTree;

        /// <summary>
        /// Инициализирует объект <see cref="BroadPhaseManager{T}"/> с заданными размерностями
        /// </summary>
        /// <param name="sceneWidth"></param>
        /// <param name="sceneHeight"></param>
        BroadPhaseManager(int sceneWidth, int sceneHeight)
        {
            _quadTree = new QuadTree<T>(0, new AABB(0, 0, sceneWidth, sceneHeight));
        }

        /// <summary>
        /// Добавляет все объекты в дерево квандрантов для последующего анализа
        /// </summary>
        /// <param name="objects"></param>
        public void Initialize(List<T> objects)
        {
            foreach (var curObject in objects)
                _quadTree.Insert(curObject);
        }

        public List<T> GetCandidates(T physObject)
        {
            var candidates = new List<T>();

            _quadTree.Retrieve(candidates, physObject);

            return candidates;
        }

        public void Clear()
        {
            _quadTree.Clear();
        }
    }
}
