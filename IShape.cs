using System;
using System.Drawing;

namespace PhysEngine
{
    /// <summary>
    /// Перечисление, определяющее возможные формы
    /// </summary>
    public enum TypeShape { CIRCLE, POLY, COUNT };
    /// <summary>
    /// Интерфейс, описывающий абстрактную форму
    /// </summary>
    public interface IShape:ICloneable
    {        
        Body Body { get; set; }
        /// <summary>
        /// Тип фигуры
        /// </summary>
        TypeShape GetType { get; }        
        /// <summary>
        /// Посчитать массу для связанного объекта
        /// </summary>
        /// <param name="origin">Ссылка на связанные объект</param>
        void ComputeMass();
        /// <summary>
        /// Инициализирует новый объект, реализующий интерфейс <see cref="IShape"/>
        /// </summary>
        void Initialize();
        /// <summary>
        /// Отрисовка фигуры внутри движка
        /// </summary>
        /// <param name="g">Объект, на котором происходит отрисовка</param>
        /// <param name="scale">Масштаб</param>
        void Draw(Graphics g, int scale);
        void SetOrient(float angleRadians);
    }
}
