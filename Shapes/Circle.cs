using System;
using PhysEngine.Collision.BroadPhase;

namespace PhysEngine.Shapes
{
    /// <summary>
    /// Представляет собой окружность
    /// </summary>
    public struct Circle: IShape
    {
        /// <summary>
        /// Радиус окружности
        /// </summary>
        public float Radius { get; set; }

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
            this.Radius = ancestor.Radius;
        }

        float IShape.CalculateArea()
        {
            if (Radius < 0)
                throw new Exception("The radius has negative value");
            return (float)Math.PI * Radius * Radius;
        }

        AABB IShape.GetBounds()
        {
            return new AABB(0, 0, Radius, Radius);
        }
    }
}
