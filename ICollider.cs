namespace PhysEngine
{
    /// <summary>
    /// Интерфейс, описывающий коллайдер для двух объектов, реализующих интерфейс <see cref="IBody"/>
    /// </summary>
    interface ICollider
    {

        IBody First { get; }

        IBody Second { get; }

        /// <summary>
        /// Проверяет, имеется ли коллизия между двумя телами
        /// </summary>
        /// <returns>Возвращает true, если между телами произошла коллизия, иначе false</returns>
        bool AreCollided();
    }
}
