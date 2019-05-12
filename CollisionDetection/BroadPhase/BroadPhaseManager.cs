using System.Collections.Generic;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    /// <summary>
    /// Fast-checker to find objects that can collide during current step.
    /// Physics objects are instantiated as their boxes, which covers their shapes.
    /// </summary>
    internal class BroadPhaseManager <T> where T: IBoxable
    {
        private readonly QuadTree<T> _quadTree;

        /// <summary>
        /// Initialize an object of <see cref="BroadPhaseManager{T}"/> with given dimensions
        /// </summary>
        /// <param name="sceneWidth">Width of the scene, which this manager is connected with</param>
        /// <param name="sceneHeight">Height of the scene, which this manager is connected with</param>
        internal BroadPhaseManager(int sceneWidth, int sceneHeight)
        {
            _quadTree = new QuadTree<T>(0, new AABB(0, 0, sceneWidth, sceneHeight));
        }

        /// <summary>
        /// Adds objects-actors of current step
        /// </summary>
        /// <param name="objects"></param>
        public void Initialize(IEnumerable<T> objects)
        {
            foreach (var curObject in objects)
                _quadTree.Insert(curObject);
        }

        /// <summary>
        /// Returns objects that might be collided with given 
        /// </summary>
        /// <param name="physObject"></param>
        /// <returns></returns>
        public List<T> GetCandidates(T physObject)
        {
            var candidates = new List<T>();

            _quadTree.Retrieve(candidates, physObject);

            return candidates;
        }

        /// <summary>
        /// Clears the list of the objects
        /// </summary>
        public void Clear()
        {
            _quadTree.Clear();
        }
    }
}
