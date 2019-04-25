using System.Collections.Generic;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    /// <summary>
    /// Делегат, определяющий должны ли сталкиваться два объекта с помощью их масок и номеров групп
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="secondObject"></param>
    /// <returns></returns>
    public delegate bool MaskFilter (IPhysObject firstObject, IPhysObject secondObject);

    /// <summary>
    /// Класс, отвечающий за "Широкую фазу" разрешения коллизий.
    /// </summary>
    class BroadPhaseManager
    {
        private QuadTree _quadTree;

        private MaskFilter _maskFilter;

        /// <summary>
        /// Удаляет из списка кандидатов объекты, боксы которых не пересекаются с боксом целевого объекта
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="targetObject"></param>
        private void FilterNonCollided(List<IPhysObject> candidates, IPhysObject targetObject)
        {
            candidates.RemoveAll(curObject => AABB.AreCollided(curObject.Box, targetObject.Box) == false ||
                _maskFilter.Invoke(curObject, targetObject)  == false);
        }

        /// <summary>
        /// Инициализирует объект <see cref="BroadPhaseManager"/> с заданными размерностями и правилами коллизий объектов
        /// </summary>
        /// <param name="sceneWidth"></param>
        /// <param name="sceneHeight"></param>
        /// <param name="maskFilter">Делегат, определяющий правила сталкивания объектов</param>
        BroadPhaseManager(int sceneWidth, int sceneHeight, MaskFilter maskFilter)
        {
            _quadTree = new QuadTree(0, new AABB(0, 0, sceneWidth, sceneHeight));
            _maskFilter = maskFilter;
        }

        /// <summary>
        /// Добавляет все объекты в дерево квандрантов для последующего анализа
        /// </summary>
        /// <param name="physObjects"></param>
        public void Initialize(List<IPhysObject> physObjects)
        {
            foreach (var curObject in physObjects)
                _quadTree.Insert(curObject);
        }

        public List<IPhysObject> GetCandidates(IPhysObject physObject)
        {
            var candidates = new List<IPhysObject>();

            _quadTree.Retrieve(candidates, physObject);
            FilterNonCollided(candidates, physObject);

            return candidates;
        }

        public void Clear()
        {
            _quadTree.Clear();
        }
    }
}
