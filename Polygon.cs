using System;
using System.Collections.Generic;

namespace PhysEngine
{
    /// <summary>
    /// Структура, представляющая форму тела "полигон"
    /// </summary>
    public struct Polygon : IShape
    {
        /// <summary>
        /// Максимальное кол-во вершин в полигоне
        /// </summary>
        public const int MAX_COUNT_VERTICES = 64;

        private Vector[] _vertices;

        private Vector[] _normals;
        
        /// <summary>
        /// Количество вершин в полигоне
        /// </summary>
        public int VerticesCount => _vertices.Length;

        /// <summary>
        /// Матрица поворота полигона
        /// </summary>
        public TMat22 MatrixOrient { get; set; }

        /// <summary>
        /// Массив нормалей к рёбрам
        /// </summary>
        public Vector[] Normals => _normals;

        /// <summary>
        /// Массив вершин полигона
        /// </summary>
        public Vector[] Vertices => _vertices;

        /// <summary>
        /// Инициализирует новый объект формы <see cref="Polygon"/> на основе уже существующей
        /// </summary>
        /// <param name="ancestor">Форма, копия которой будет создана</param>
        public Polygon(Polygon ancestor)
        {
            this._vertices = new Vector[ancestor.VerticesCount];
            this._normals = new Vector[ancestor.VerticesCount];
            this.MatrixOrient = new TMat22(ancestor.MatrixOrient);
            for (int i = 0; i < _vertices.Length; ++i)
            {
                this._vertices[i] = ancestor._vertices[i];
                this._normals[i] = ancestor._normals[i];
            }
        }

        /// <summary>
        /// Инициализирует новый объект формы <see cref="Polygon"/> на основе массива вершин
        /// </summary>
        /// <param name="vertices">Массив вершин полигона</param>
        public Polygon(Vector[] vertices)
        {
            if (vertices.Length < 3 || vertices.Length > MAX_COUNT_VERTICES)
                throw new Exception("The incorrect number of vertices was given");
            MatrixOrient = new TMat22(0);
            _vertices = new Vector[vertices.Length];
            _normals = new Vector[vertices.Length];
        }

        /// <summary>
        /// Инициализирует новый полигон в форме прямоугольника с заданной высотой и шириной
        /// </summary>
        /// <param name="width">Ширина прямоугольника</param>
        /// <param name="height">Высота прямоугольника</param>
        public Polygon(float width, float height)
        {
            if (width <= 0 || height <= 0)
                throw new Exception("The incorrect values of dimensions was given");
            var halfWidth = width / 2.0f;
            var halfHeight = height / 2.0f;
            MatrixOrient = new TMat22(0);
            _vertices = new Vector[4];
            _normals = new Vector[4];
            _vertices[0].Set(-halfWidth, -halfHeight);
            _vertices[1].Set(halfWidth, -halfHeight);
            _vertices[2].Set(halfWidth, halfHeight);
            _vertices[3].Set(-halfWidth, halfHeight);
            _normals[0].Set(0.0f, -1.0f);
            _normals[1].Set(1.0f, 0.0f);
            _normals[2].Set(0.0f, 1.0f);
            _normals[3].Set(-1.0f, 0.0f);
        }

        /// <summary>
        /// Поворот фигуры
        /// </summary>
        public void SetOrient(float angle)
        {
            MatrixOrient.Set(angle);
        }

        /// <summary>
        /// Получает самую дальнюю вершину полигона в заданном направлении
        /// </summary>
        /// <param name="direction">Вектор направления</param>
        /// <returns></returns>
        public Vector GetSupport(Vector direction)
        {
            var bestProjection = Vector.DotProduct(Vertices[0], direction);
            var bestVertex = Vertices[0];

            for (int i = 1; i < VerticesCount; ++i)
            {
                float projection = Vector.DotProduct(Vertices[i], direction);
                if (projection > bestProjection)
                {
                    bestProjection = projection;
                    bestVertex = Vertices[i];
                }
            }
            return bestVertex;
        }

        float IShape.CalculateArea()
        {
            var area = 0.0f;
            for (var i = 0; i < VerticesCount; ++i)
            {
                var next = i + 1 == VerticesCount ? 0 : i + 1;
                area += Math.Abs((Vertices[i].X + Vertices[next].X) * (Vertices[i].Y - Vertices[next].Y));
            }
            return area / 2.0f;
        }

        /// <summary>
        /// Проверка ориентированной площади треугольника (по часовой стрелке)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool ClockWiseArea(Vector a, Vector b, Vector c)
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
        private bool CounterClockWiseArea(Vector a, Vector b, Vector c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0;
        }
        /// <summary>
        /// Преобразует множество точек в полигон 
        /// </summary>
        /// <param name="vertices"></param>
        public void Set(Vector [] vertices)
        {
            Array.Sort(vertices);
            // Выделяем самую левую нижнюю вершину и самую правую верхнюю
            Vector pntA = vertices[0], pntB = vertices[vertices.Length - 1];
            // Прямая, проходящая через эти две прямые делит множество точек будущего полигона на два множества
            List<Vector> upSet = new List<Vector>(), downSet = new List<Vector>();
            upSet.Add(pntA);
            downSet.Add(pntA);
            for (int i = 1; i < vertices.Length; ++i)
            {
                // Обновляем множество верхних точек
                if (i == vertices.Length - 1 || ClockWiseArea(pntA, vertices[i], pntB))
                {
                    while (upSet.Count >= 2 && !ClockWiseArea(upSet[upSet.Count-2], upSet[upSet.Count-1], vertices[i]))
                    {
                        upSet.RemoveAt(upSet.Count - 1);
                    }
                    upSet.Add(vertices[i]);
                }
                // Обновляем множество нижних
                if (i == vertices.Length - 1 || CounterClockWiseArea(pntA, vertices[i], pntB))
                {
                    while (downSet.Count >= 2 && !CounterClockWiseArea(downSet[downSet.Count - 2], downSet[downSet.Count - 1], 
                        vertices[i]))
                    {
                        downSet.RemoveAt(downSet.Count - 1);
                    }
                    downSet.Add(vertices[i]);
                }
            }
            // Собираем полученные вершины МВО
            _vertices = new Vector[upSet.Count + downSet.Count - 2];
            int iter = 0;
            for (int j = upSet.Count -1; j >= 0; --j, ++iter)
                _vertices[iter] = upSet[j];
            for (int j = 1; j < downSet.Count - 1; ++j, ++iter)
                _vertices[iter] = downSet[j];
            // Рассчитываем нормали к рёбрам
            _normals = new Vector[_vertices.Length];
            for (int i = 0; i < _vertices.Length; ++i)
            {
                int snd = i + 1;
                if (snd == _vertices.Length)
                    snd = 0;
                Vector face = _vertices[snd] - _vertices[i];
                // Проверяем на нулевое ребро
                if (face.LengthSquared < TScene.EPS * TScene.EPS)
                    throw new Exception("Длина ребра полигона равна 0");

                _normals[i] = new Vector(face.Y, -face.X);
                _normals[i].Normalize();
            }
        }
    }
}
