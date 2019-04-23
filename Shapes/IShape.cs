using PhysEngine.Collision.BroadPhase;

namespace PhysEngine.Shapes
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
        AABB GetBounds();
    }
}
