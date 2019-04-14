using System;
using System.Collections.Generic;
using System.Drawing;

namespace PhysEngine
{
    /// <summary>
    /// Представляет форму тела "многоугольник"
    /// </summary>
    public class TPolygon : IShape
    {
        /// <summary>
        /// Максимальное количество вершин в <see cref="TPolygon"/>
        /// </summary>
        public const int MAX_COUNT_VERTICES = 64;
        private Vector2D[] vertices;
        private Vector2D[] normals;        
        /// <summary>
        /// Тип формы
        /// </summary>
        TypeShape IShape.GetType => TypeShape.POLY;
        /// <summary>
        /// Количество вершин в <see cref="TPolygon"/>
        /// </summary>
        public int VerticesCount => vertices.Length;
        /// <summary>
        /// Матрица ориентации <see cref="TPolygon"/>
        /// </summary>
        public TMat22 MatrOrient { get; set; }
        /// <summary>
        /// Массив нормалей к рёбрам
        /// </summary>
        public Vector2D[] Normals => normals;
        /// <summary>
        /// Массив вершин полигона
        /// </summary>
        public Vector2D[] Vertices => vertices;
        /// <summary>
        /// Ссылка на тело, имеющее данную форму
        /// </summary>
        public TBody Body { get; set; }
        public TPolygon()
        {
            MatrOrient = new TMat22(0);
        }
        /// <summary>
        /// Проверка ориентированной площади треугольника (по часовой стрелке)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool ClockWiseArea(Vector2D a, Vector2D b, Vector2D c)
        {
            return (b.X - a.X)*(c.Y - a.Y) - (b.Y - a.Y)*(c.X - a.X) < 0;
        }
        /// <summary>
        /// Проверка ориентированной площади треугольника (против часовой стрелки)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool CounterClockWiseArea(Vector2D a, Vector2D b, Vector2D c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0;
        }
        /// <summary>
        /// Преобразует <see cref="TPolygon"/> в прямоугольник, определяемый половинками высоты и ширины
        /// </summary>
        /// <param name="halfWidth"></param>
        /// <param name="halfHeight"></param>
        public void SetBox(float halfWidth, float halfHeight)
        {
            vertices = new Vector2D[4];
            normals = new Vector2D[4];
            vertices[0].Set(-halfWidth, -halfHeight);
            vertices[1].Set(halfWidth, -halfHeight);
            vertices[2].Set(halfWidth, halfHeight);
            vertices[3].Set(-halfWidth, halfHeight);
            normals[0].Set(0.0f, -1.0f);
            normals[1].Set(1.0f, 0.0f);
            normals[2].Set(0.0f, 1.0f);
            normals[3].Set(-1.0f, 0.0f);
        }
        /// <summary>
        /// Преобразует множество точек в полигон 
        /// </summary>
        /// <param name="valueVertices"></param>
        public void Set(Vector2D [] valueVertices)
        {
            //Если вершин меньше 3, то мы не сможем построить полигон
            if (valueVertices.Length < 3 || valueVertices.Length > MAX_COUNT_VERTICES)
                throw new Exception("Некорректное число вершин в полигоне");
            Array.Sort(valueVertices);
            // Выделяем самую левую нижнюю вершину и самую правую верхнюю
            Vector2D pntA = valueVertices[0], pntB = valueVertices[valueVertices.Length - 1];
            // Прямая, проходящая через эти две прямые делит множество точек будущего полигона на два множества
            List<Vector2D> upSet = new List<Vector2D>(), downSet = new List<Vector2D>();
            upSet.Add(pntA);
            downSet.Add(pntA);
            for (int i = 1; i < valueVertices.Length; ++i)
            {
                // Обновляем множество верхних точек
                if (i == valueVertices.Length - 1 || ClockWiseArea(pntA, valueVertices[i], pntB))
                {
                    while (upSet.Count >= 2 && !ClockWiseArea(upSet[upSet.Count-2], upSet[upSet.Count-1], valueVertices[i]))
                    {
                        upSet.RemoveAt(upSet.Count - 1);
                    }
                    upSet.Add(valueVertices[i]);
                }
                // Обновляем множество нижних
                if (i == valueVertices.Length - 1 || CounterClockWiseArea(pntA, valueVertices[i], pntB))
                {
                    while (downSet.Count >= 2 && !CounterClockWiseArea(downSet[downSet.Count - 2], downSet[downSet.Count - 1], 
                        valueVertices[i]))
                    {
                        downSet.RemoveAt(downSet.Count - 1);
                    }
                    downSet.Add(valueVertices[i]);
                }
            }
            // Собираем полученные вершины МВО
            vertices = new Vector2D[upSet.Count + downSet.Count - 2];
            int iter = 0;
            for (int j = upSet.Count -1; j >= 0; --j, ++iter)
                vertices[iter] = upSet[j];
            for (int j = 1; j < downSet.Count - 1; ++j, ++iter)
                vertices[iter] = downSet[j];
            // Рассчитываем нормали к рёбрам
            normals = new Vector2D[vertices.Length];
            for (int i = 0; i < vertices.Length; ++i)
            {
                int snd = i + 1;
                if (snd == vertices.Length)
                    snd = 0;
                Vector2D face = vertices[snd] - vertices[i];
                // Проверяем на нулевое ребро
                if (face.LengthSquared < TScene.EPS * TScene.EPS)
                    throw new Exception("Длина ребра полигона равна 0");

                normals[i] = new Vector2D(face.Y, -face.X);
                normals[i].Normalize();
            }
        }
        /// <summary>
        /// Инициализирует новый объект формы <see cref="TPolygon"/> с теми же параметрами
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            TPolygon clone = new TPolygon();
            clone.vertices = new Vector2D[VerticesCount];
            clone.normals = new Vector2D[VerticesCount];
            clone.MatrOrient = new TMat22(MatrOrient);
            for (int i = 0; i < vertices.Length; ++i)
            {
                clone.vertices[i] = vertices[i];
                clone.normals[i] = normals[i];
            }
            return clone;
        }
        /// <summary>
        /// Рассчёт массы для тела с формой <see cref="TPolygon"/>
        /// </summary>
        public void ComputeMass()
        {
            // Посчитать центроид и момент инерции
            Vector2D centroid = new Vector2D(0.0f, 0.0f);
            float area = 0.0f;
            float inertia = 0.0f;
            const float INV3 = 1.0f / 3.0f;
            for (var i = 0; i < VerticesCount; ++i)
            {
                // Вершины треугольника, треья вершина подразумевается как (0, 0)
                Vector2D p1 = new Vector2D(vertices[i]);
                var j = i + 1;
                if (j == VerticesCount )
                    j = 0;

                Vector2D p2 = new Vector2D(vertices[j]);

                float distance = p1^p2;
                float triangleArea = 0.5f * distance;

                area += triangleArea;

                // Используем площадь, чтобы взвесить среднее значение по центроиду, а не только положение вершины
                centroid += triangleArea * INV3 * (p1 + p2);

                float intx2 = p1.X * p1.X + p2.X * p1.X + p2.X * p2.X;
                float inty2 = p1.Y * p1.Y + p2.Y * p1.Y + p2.Y * p2.Y;
                inertia += (0.25f * INV3 * distance) * (intx2 + inty2);
            }
            centroid *= 1.0f / area;

            // Перевод вершин в систему координат центроида (центроид становится точкой (0;0)
            for (var i = 0; i < VerticesCount; ++i)
                vertices[i] -= centroid;
            Body.Mass = Body.GetMaterial.Density * area;
            Body.Inertia = Body.GetMaterial.Density * inertia;
        }
        /// <summary>
        /// Поворот фигуры
        /// </summary>
        public void SetOrient(float valueRadians)
        {
            MatrOrient.Set(valueRadians);
        }
        /// <summary>
        /// Функция отрисовки формы на экране с помощью инструментов GDI+
        /// </summary>
        public void Draw(Graphics g, int scale)
        {
            PointF[] pnts = new PointF[VerticesCount];
            for (int i = 0; i < pnts.Length; ++i)
            {
                Vector2D vertex = MatrOrient * vertices[i] + Body.Position;
                vertex *= scale;
                pnts[i] = new PointF(vertex.X, vertex.Y);
            }
            g.DrawPolygon(new Pen(Brushes.Black), pnts);
        }
        /// <summary>
        /// Производит действия по расчёту массы объекта, связываемого с формой <see cref="TPolygon"/>
        /// </summary>
        /// <param name="origin"></param>
        public void Initialize()
        {
            if (Body == null)
                throw new Exception("К форме не было привязано никакого тела");
            ComputeMass();
        }
        /// <summary>
        /// Получает самую дальнюю точку объекта класса <see cref="TPolygon"/> в заданном направлении
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Vector2D GetSupport(Vector2D direction)
        {
            float bestProjection = vertices[0] * direction;
            Vector2D bestVertex = vertices[0];

            for (int i = 1; i < VerticesCount; ++i)
            {
                float projection = vertices[i] * direction;
                if (projection > bestProjection)
                {
                    bestProjection = projection;
                    bestVertex = vertices[i];
                }
            }
            return bestVertex;
        }
    }
}
