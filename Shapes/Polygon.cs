using System;

namespace PhysEngine.Shapes
{
    /// <summary>
    /// Структура, представляющая форму тела "полигон"
    /// </summary>
    public struct Polygon : IShape
    {
        /// <summary>
        /// Максимальное кол-во вершин в полигоне
        /// </summary>
        public const int MAX_COUNT_VERTICES = 16;

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
                Vertices[i] = ancestor.Vertices[i];
                Normals[i] = ancestor.Normals[i];
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

            var polygonSetter = new PolygonSetter(vertices);
            polygonSetter.SetPolygon(out _vertices, out _normals);
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
            Vertices[0].Set(-halfWidth, -halfHeight);
            Vertices[1].Set(halfWidth, -halfHeight);
            Vertices[2].Set(halfWidth, halfHeight);
            Vertices[3].Set(-halfWidth, halfHeight);
            Normals[0].Set(0.0f, -1.0f);
            Normals[1].Set(1.0f, 0.0f);
            Normals[2].Set(0.0f, 1.0f);
            Normals[3].Set(-1.0f, 0.0f);
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
                var next = i + 1;
                if (next == VerticesCount)
                    next = 0;

                area += Math.Abs((Vertices[i].X + Vertices[next].X) * (Vertices[i].Y - Vertices[next].Y));
            }

            return area / 2.0f;
        }
    }
}
