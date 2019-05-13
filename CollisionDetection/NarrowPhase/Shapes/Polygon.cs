using System;
using PhysEngine.Common;
using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.CollisionDetection.NarrowPhase.Shapes
{
    /// <summary>
    /// Structure that implements the polygon shape
    /// </summary>
    public partial struct Polygon : IShape
    {
        public const int MaxCountVertices = 16;
        
        /// <summary>
        /// Number of vertices in the polygon
        /// </summary>
        public int VerticesCount => Vertices.Length;
        
        public Mat22 MatrixOrient { get; set; }

        /// <summary>
        /// Array of normals to the edges
        /// </summary>
        public Vector[] Normals { get; }
        
        public Vector[] Vertices { get; }

        /// <summary>
        /// Initialize new object of <see cref="Polygon"/> from the array of vertices
        /// </summary>
        /// <param name="vertices"></param>
        public Polygon(Vector[] vertices)
        {
            if (vertices.Length < 3 || vertices.Length > MaxCountVertices)
                throw new Exception("The incorrect number of vertices was given");

            MatrixOrient = new Mat22(0);

            var polygonSetter = new PolygonSetter(vertices);
            polygonSetter.SetPolygon(out var tmpVertices, out var tmpNormals);

            Vertices = tmpVertices;
            Normals = tmpNormals;
        }

        /// <summary>
        /// Initialize new object of <see cref="Polygon"/> shaped as a rectangle with given width and height
        /// </summary>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public Polygon(float width, float height)
        {
            if (width <= 0)
                throw new Exception("The width has a non-positive value");

            if (height <= 0)
                throw new Exception("The height has a non-positive value");

            var halfWidth = width / 2.0f;
            var halfHeight = height / 2.0f;
            MatrixOrient = new Mat22(0);
            Vertices = new Vector[4];
            Normals = new Vector[4];
            Vertices[0].Set(-halfWidth, -halfHeight);
            Vertices[1].Set(halfWidth, -halfHeight);
            Vertices[2].Set(halfWidth, halfHeight);
            Vertices[3].Set(-halfWidth, halfHeight);
            Normals[0].Set(0.0f, -1.0f);
            Normals[1].Set(1.0f, 0.0f);
            Normals[2].Set(0.0f, 1.0f);
            Normals[3].Set(-1.0f, 0.0f);
        }
        
        public void SetOrient(float angle)
        {
            MatrixOrient.Set(angle);
        }

        /// <summary>
        /// Gets the most far vertex of the polygon in the given direction
        /// </summary>
        /// <param name="direction">Vector of the direction</param>
        /// <returns></returns>
        public Vector GetSupport(Vector direction)
        {
            var bestProjection = Vector.DotProduct(Vertices[0], direction);
            var bestVertex = Vertices[0];

            for (var i = 1; i < VerticesCount; ++i)
            {
                var projection = Vector.DotProduct(Vertices[i], direction);
                if (projection > bestProjection)
                {
                    bestProjection = projection;
                    bestVertex = Vertices[i];
                }
            }
            return bestVertex;
        }

        public float CalculateArea()
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

        public AABB GetBox()
        {
            var leftUpper = Vertices[0];
            var rightLower = Vertices[0];
            for (var i = 1; i < VerticesCount; ++i)
            {
                if (leftUpper.X > Vertices[i].X)
                    leftUpper.X = Vertices[i].X;
                if (leftUpper.Y > Vertices[i].Y)
                    leftUpper.Y = Vertices[i].Y;
                if (rightLower.X < Vertices[i].X)
                    rightLower.X = Vertices[i].X;
                if (rightLower.Y < Vertices[i].Y)
                    rightLower.Y = Vertices[i].Y;
            }
            return new AABB(leftUpper, rightLower);
        }
    }
}
