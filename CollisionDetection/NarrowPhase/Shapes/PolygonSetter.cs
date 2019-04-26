using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Класс, представляющий собой метод преобразования массива точек в полигон с помощью построения МВО.
    /// Алгоритм Грэхэма-Эндрю: https://e-maxx.ru/algo/convex_hull_graham
    /// </summary>
    class PolygonSetter
    {
        private Vector[] inputVertices;
        private Vector[] outputVertices;
        private Vector[] normals;
        private List<Vector> lowerSet;
        private List<Vector> upperSet;
        private Vector pointLeftUpper;
        private Vector pointRightLower;

        /// <summary>
        /// Проверка на отрицательность ориентированной площади треугольника (по часовой стрелке)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsClockwiseAreaNegative(Vector a, Vector b, Vector c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) < 0;
        }

        /// <summary>
        /// Проверка на положительность ориентированной площади треугольника (против часовой стрелки)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsCounterClockwiseAreaPositive(Vector a, Vector b, Vector c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0;
        }

        /// <summary>
        /// Обновление набора вершин выше разделяющей оси
        /// </summary>
        /// <param name="index">Текущая вершина для обновления</param>
        private void UpdateUpperSet(int index)
        {
            if (index == inputVertices.Length - 1 || IsClockwiseAreaNegative(pointLeftUpper, inputVertices[index], pointRightLower))
            {
                while (upperSet.Count >= 2 && !IsClockwiseAreaNegative(upperSet[upperSet.Count - 2], upperSet.Last(), inputVertices[index]))
                {
                    upperSet.RemoveAt(upperSet.Count - 1);
                }
                upperSet.Add(inputVertices[index]);
            }
        }

        /// <summary>
        /// Обновление набора вершин ниже разделяющей оси
        /// </summary>
        /// <param name="index"></param>
        private void UpdateLowerSet(int index)
        {
            if (index == inputVertices.Length - 1 || IsCounterClockwiseAreaPositive(pointLeftUpper, inputVertices[index], pointRightLower))
            {
                while (lowerSet.Count >= 2 && !IsCounterClockwiseAreaPositive(lowerSet[lowerSet.Count - 2], lowerSet.Last(),
                    inputVertices[index]))
                {
                    lowerSet.RemoveAt(lowerSet.Count - 1);
                }
                lowerSet.Add(inputVertices[index]);
            }
        }

        /// <summary>
        /// Получает итоговые вершины МВО в порядке против часовой стрелки
        /// </summary>
        private void ExtractVerticesFromSets()
        {
            outputVertices = new Vector[upperSet.Count + lowerSet.Count - 2];
            var iter = 0;
            for (var j = upperSet.Count - 1; j >= 0; --j, ++iter)
                outputVertices[iter] = upperSet[j];
            for (var j = 1; j < lowerSet.Count - 1; ++j, ++iter)
                outputVertices[iter] = lowerSet[j];
        }

        /// <summary>
        /// Находит МВО алгоритмом Грэхэма-Эндрю
        /// </summary>
        private void FindMinConvexHull()
        {
            Array.Sort(inputVertices, new DefaultVectorComparator());

            pointLeftUpper = inputVertices.First();
            pointRightLower = inputVertices.Last();

            upperSet.Add(pointLeftUpper);
            lowerSet.Add(pointLeftUpper);

            for (var index = 1; index < inputVertices.Length; ++index)
            {
                UpdateUpperSet(index);
                UpdateLowerSet(index);
            }

            ExtractVerticesFromSets();
        }

        /// <summary>
        /// Рассчитывает массив нормалей для полученной МВО
        /// </summary>
        private void ComputeNormals()
        {
            normals = new Vector[outputVertices.Length];
            for (var i = 0; i < normals.Length; ++i)
            {
                var next = i + 1;
                if (next == normals.Length)
                    next = 0;
                Vector face = outputVertices[next] - outputVertices[i];

                if (face.LengthSquared < PhysEngineDefaults.EPS * PhysEngineDefaults.EPS)
                    throw new Exception("Длина ребра полигона равна 0");

                normals[i] = new Vector(face.Y, -face.X);
                normals[i].Normalize();
            }
        }

        public PolygonSetter(Vector[] vertices)
        {
            inputVertices = vertices;
        }

        public void SetPolygon(out Vector[] polyVertices, out Vector[] polyNormals)
        {
            FindMinConvexHull();
            ComputeNormals();

            polyVertices = outputVertices;
            polyNormals = normals;
        }
    }
}
