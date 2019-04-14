using System;
using System.Drawing;

namespace PhysEngine
{
    /// <summary>
    /// Представляет собой окружность
    /// </summary>
    public class TCircle: IShape
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
        /// Связанное с формой тело
        /// </summary>
        public TBody Body { get; set; }
        /// <summary>
        /// Инициализирует объект структуры <see cref="TCircle"/>
        /// </summary>
        /// <param name="valueRadius">Радиус новой окружности</param>
        public TCircle(float valueRadius)
        {
            Radius = valueRadius;
        }
        /// <summary>
        /// Тип формы
        /// </summary>
        TypeShape IShape.GetType => TypeShape.CIRCLE;
        /// <summary>
        /// Инициализирует новый объект формы <see cref="TCircle"/> с тем же радиусом
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TCircle(Radius);
        }
        /// <summary>
        /// Рассчитывает массу объекта, заданного формой <see cref="TCircle"/>
        /// </summary>
        /// <param name="origin">Ссылка на объект</param>
        public void ComputeMass()
        {
            Body.Mass = PI * Radius * Radius * Body.GetMaterial.Density;
            Body.Inertia = Body.Mass * Radius * Radius;
        }
        /// <summary>
        /// Функция отрисовки формы на экране с помощью инструментов GDI+ 
        /// </summary>
        public void Draw(Graphics g, int scale)
        {
            PointF leftUpper = new PointF((Body.Position.X - Radius)*scale, (Body.Position.Y - Radius)*scale);
            RectangleF box = new RectangleF(leftUpper, new SizeF(Radius * 2 * scale, Radius * 2 * scale));
            g.DrawEllipse(new Pen(Brushes.Black), box);
        }
        /// <summary>
        /// Производит действия по расчёту массы объекта, связываемого с формой <see cref="TCircle"/>
        /// </summary>
        public void Initialize()
        {
            if (Body == null)
                throw new Exception("К форме не было привязано никакого тела");
            ComputeMass();
        }
        public void SetOrient(float angleRadians)
        {
            
        }
    }
}
