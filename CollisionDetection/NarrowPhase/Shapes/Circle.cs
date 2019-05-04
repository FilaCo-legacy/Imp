using System;
using PhysEngine.CollisionDetection.BroadPhase;
using PhysEngine.Common;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Представляет собой окружность
    /// </summary>
    public struct Circle: IShape
    {
        /// <summary>
        /// Радиус окружности
        /// </summary>
        public float Radius { get; }

        /// <summary>
        /// Инициализирует объект структуры <see cref="Circle"/>
        /// </summary>
        /// <param name="radius">Радиус новой окружности</param>
        public Circle(float radius)
        {
            if (radius < 0)
                throw new Exception("The radius has negative value");
            Radius = radius;
        }

        /// <summary>
        /// Инициализирует новый объект формы <see cref="Circle"/> с тем же радиусом
        /// </summary>
        /// <returns></returns>
        public Circle(Circle ancestor)
        { 
            Radius = ancestor.Radius;
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
