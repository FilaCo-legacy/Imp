using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Интерфейс, описывающий форму объекта
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// Вычисляет площадь формы
        /// </summary>
        /// <returns></returns>
        float CalculateArea();

        /// <summary>
        /// Возвращает прямоугольник, ограничивающий форму, левый верхний угол которого помещён в начало координат
        /// </summary>
        /// <returns></returns>
        AABB GetBox();

        /// <summary>
        /// Рассчёт массы физического тела, описываемого данной формой
        /// </summary>
        /// <param name="density">Плотность материала тела</param>
        /// <returns></returns>
        float ComputeMass(float density);

        /// <summary>
        /// Рассчёт инерции физического тела, описываемого данной формой
        /// </summary>
        /// <param name="mass">Масса тела</param>
        /// <returns></returns>
        float ComputeInertia(float mass);
    }
}
