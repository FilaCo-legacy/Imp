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
    }
}
