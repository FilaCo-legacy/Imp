using System;

namespace PhysEngine
{
    /// <summary>
    /// Представляет собой окружность
    /// </summary>
    public struct Circle: IShape
    {
        /// <summary>
        /// Константа PI
        /// </summary>
        public const float PI = 3.14159f;
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
        public Circle(Circle pref)
        {
            this.Radius = pref.Radius;
        }
        float IShape.CalculateArea()
        {
            if (Radius < 0)
                throw new Exception("The radius has negative value");
            return PI * Radius * Radius;
        }
    }
}
