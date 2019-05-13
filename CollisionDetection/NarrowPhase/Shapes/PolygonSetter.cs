using System;
using System.Collections.Generic;
using System.Linq;
using PhysEngine.Common;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Class that implements the algorithm of building a polygon from array of points.
    /// https://e-maxx.ru/algo/convex_hull_graham
    /// </summary>
    class PolygonSetter
    {
        private readonly Vector[] inputVertices;
        private Vector[] outputVertices;
        private Vector[] normals;
        private readonly List<Vector> lowerSet;
        private readonly List<Vector> upperSet;
        private Vector pointLeftUpper;
        private Vector pointRightLower;
        
        private static bool IsClockwiseAreaNegative(Vector a, Vector b, Vector c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) < 0;
        }
        
        private static bool IsCounterClockwiseAreaPositive(Vector a, Vector b, Vector c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0;
        }

        /// <summary>
        /// Updates a set of points that are located upper than the separating axis
        /// </summary>
        /// <param name="index">Index of the input vertex</param>
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
        /// Updates a set of points that are located lower than the separating axis
        /// </summary>
        /// <param name="index">Index of the input vertex</param>
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
        /// Gets vertices of the MCH in the counter clockwise order
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
        
        private void FindMinConvexHull()
        {
            lowerSet.Clear();
            upperSet.Clear();
            
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
                    throw new Exception("Length of the edge was 0");

                normals[i] = new Vector(face.Y, -face.X);
                normals[i].Normalize();
            }
        }

        public PolygonSetter(Vector[] vertices)
        {
            inputVertices = vertices;
            lowerSet = new List<Vector>();
            upperSet = new List<Vector>();
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
