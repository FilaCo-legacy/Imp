﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public class DefaultVectorComparator : IComparer<Vector>
    {
        int IComparer<Vector>.Compare(Vector a, Vector b)
        {
            if (Math.Abs(a.X - b.X) < PhysEngineConsts.EPS)
            {
                if (Math.Abs(a.Y - b.Y) < PhysEngineConsts.EPS)
                    return 0;
                return a.Y.CompareTo(b.Y);
            }
            return a.X.CompareTo(b.X);
        }
    }

    /// <summary>
    /// Класс, представляющий собой метод преобразования массива точек в полигон с помощью построения МВО.
    /// Алгоритм Грэхэма-Эндрю: https://e-maxx.ru/algo/convex_hull_graham
    /// </summary>
    class PolygonSetter
    {
        private Vector[] inputVertices;
        private Vector[] outputVertices;
        private Vector[] normals;
        private List<Vector> downSet;
        private List<Vector> upSet;
        private Vector pointLeftDown;
        private Vector pointRightUp;

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
        private void UpdateUpSet(int index)
        {
            if (index == inputVertices.Length - 1 || IsClockwiseAreaNegative(pointLeftDown, inputVertices[index], pointRightUp))
            {
                while (upSet.Count >= 2 && !IsClockwiseAreaNegative(upSet[upSet.Count - 2], upSet.Last(), inputVertices[index]))
                {
                    upSet.RemoveAt(upSet.Count - 1);
                }
                upSet.Add(inputVertices[index]);
            }
        }

        /// <summary>
        /// Обновление набора вершин ниже разделяющей оси
        /// </summary>
        /// <param name="index"></param>
        private void UpdateDownSet(int index)
        {
            if (index == inputVertices.Length - 1 || IsCounterClockwiseAreaPositive(pointLeftDown, inputVertices[index], pointRightUp))
            {
                while (downSet.Count >= 2 && !IsCounterClockwiseAreaPositive(downSet[downSet.Count - 2], downSet.Last(),
                    inputVertices[index]))
                {
                    downSet.RemoveAt(downSet.Count - 1);
                }
                downSet.Add(inputVertices[index]);
            }
        }

        /// <summary>
        /// Получает итоговые вершины МВО в порядке против часовой стрелки
        /// </summary>
        private void ExtractVerticesFromSets()
        {
            outputVertices = new Vector[upSet.Count + downSet.Count - 2];
            var iter = 0;
            for (var j = upSet.Count - 1; j >= 0; --j, ++iter)
                outputVertices[iter] = upSet[j];
            for (var j = 1; j < downSet.Count - 1; ++j, ++iter)
                outputVertices[iter] = downSet[j];
        }

        /// <summary>
        /// Находит МВО алгоритмом Грэхэма-Эндрю
        /// </summary>
        private void FindMinConvexHull()
        {
            Array.Sort(inputVertices, new DefaultVectorComparator());

            pointLeftDown = inputVertices.First();
            pointRightUp = inputVertices.Last();

            upSet.Add(pointLeftDown);
            downSet.Add(pointLeftDown);

            for (var index = 1; index < inputVertices.Length; ++index)
            {
                UpdateUpSet(index);
                UpdateDownSet(index);
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

                if (face.LengthSquared < PhysEngineConsts.EPS * PhysEngineConsts.EPS)
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
