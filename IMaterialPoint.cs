namespace PhysEngine
{
    /// <summary>
    /// Интерфейс, представляющий материальную точку
    /// </summary>
    public interface IMaterialPoint
    {
        /// <summary>
        /// Масса материальной точки
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// Инерция материальной точки
        /// </summary>
        float Inertia { get; }

        /// <summary>
        /// Величина, обратная массе материальной точки.
        /// Необходима для выполнения более быстрых вычислений
        /// </summary>
        float InverseMass { get; }

        /// <summary>
        /// Величина, обратная инерции материальной точки.
        /// Необходима для выполнения более быстрых вычислений
        /// </summary>
        float InverseInertia { get; }

        /// <summary>
        /// Компонента, содержащая информацию о векторной составляющей материальной точки
        /// </summary>
        IVectorComponent VectorComponent { get; set; }
    }
}
