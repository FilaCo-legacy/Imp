using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Interface that defines the shape of a physics object
    /// </summary>
    public interface IShape
    {
        float CalculateArea();

        /// <summary>
        /// Returns box, which cover the whole shape of the object
        /// </summary>
        /// <returns></returns>
        AABB GetBox();

        /// <summary>
        /// Computes mass of the body with this shape
        /// </summary>
        /// <param name="density">Density of the body's material</param>
        /// <returns></returns>
        float ComputeMass(float density);

        /// <summary>
        /// Computes inertia of the body with this shape and mass
        /// </summary>
        /// <param name="mass">Mass of the body</param>
        /// <returns></returns>
        float ComputeInertia(float mass);
    }
}
