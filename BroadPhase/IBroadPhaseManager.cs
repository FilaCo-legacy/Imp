using System.Collections.Generic;

namespace ImpLite.BroadPhase
{
    /// <summary>
    /// Provides an interface for classes that are responsible for a broad-phase check
    /// </summary>
    /// <typeparam name="TObject">Type of objects in scene</typeparam>
    public interface IBroadPhaseManager<TObject> where TObject : IBoxable
    {
        /// <summary>
        /// Starts a broad-phase check for some collection of objects
        /// </summary>
        /// <param name="objects"></param>
        void Execute(IEnumerable<TObject> objects);

        /// <summary>
        /// Finds all neighbours of some object
        /// </summary>
        /// <param name="physObject">Target object</param>
        /// <returns></returns>
        List<TObject> GetNeighbours(TObject physObject);

        /// <summary>
        /// Removes all objects from this manager store
        /// </summary>
        void Clear();
    }
}