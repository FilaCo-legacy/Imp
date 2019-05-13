using System;
using PhysEngine.CollisionDetection.BroadPhase;
using PhysEngine.Common;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Structure that implements the circle shape
    /// </summary>
    public struct Circle: IShape
    {
        public float Radius { get; }

        /// <summary>
        /// Initialize the object of <see cref="Circle"/> with given radius
        /// </summary>
        /// <param name="radius">The radius of a new circle</param>
        public Circle(float radius)
        {
            if (radius < 0)
                throw new Exception("The radius has negative value");
            Radius = radius;
        }

        public float CalculateArea()
        {
            if (Radius < 0)
                throw new Exception("The radius has negative value");
            return (float)Math.PI * Radius * Radius;
        }

        public AABB GetBox()
        {
            return new AABB(0, 0, Radius, Radius);
        }

        public float ComputeMass(float density)
        {
            return CalculateArea() * density;
        }

        public float ComputeInertia(float mass)
        {
            return mass * Radius * Radius;
        }
    }
}
