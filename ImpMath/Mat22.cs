using System;
using System.Numerics;

namespace Imp.ImpMath
{
    /// <summary>
    /// Represents a matrix 2x2
    /// </summary>
    public struct Matrix2x2
    {
        private readonly float[,] _matrix;
        
        public float this [int i, int j] { get => _matrix[i,j]; set => _matrix[i, j] = value; }
        
        public Matrix2x2(Vector2 fstRow, Vector2 sndRow)
        {
            _matrix = new float[2, 2];
            _matrix[0, 0] = fstRow.X;
            _matrix[0, 1] = fstRow.Y;
            _matrix[1, 0] = sndRow.X;
            _matrix[1, 1] = sndRow.Y;
        }
        
        public Matrix2x2(float angle)
        {
            _matrix = new float[2, 2];
            Set(angle);
        }
        
        public Matrix2x2(float m00, float m01, float m10, float m11)
        {
            _matrix = new float[2, 2];
            _matrix[0, 0] = m00;
            _matrix[0, 1] = m01;
            _matrix[1, 0] = m10;
            _matrix[1, 1] = m11;
        }
        
        public Matrix2x2(Matrix2x2 pref)
        {
            _matrix = new float[2, 2];
            
            for (var i = 0; i < 2; ++i)
                for (var j = 0; j < 2; ++j)
                    _matrix[i, j] = pref[i, j];
        }
        
        public void Set(float angleRad)
        {
            var cosAngle = (float)Math.Cos(angleRad);
            var sinAngle = (float)Math.Sin(angleRad);
            _matrix[0, 0] = cosAngle;
            _matrix[0, 1] = -sinAngle;
            _matrix[1, 0] = sinAngle;
            _matrix[1, 1] = cosAngle;
        }
        
        public Matrix2x2 Transpose()
        {
            return new Matrix2x2(_matrix[0, 0], _matrix[1, 0], _matrix[0, 1], _matrix[1, 1]);
        }
        
        public static Vector2 operator * (Matrix2x2 matr, Vector2 rhs)
        {
            return new Vector2(matr[0, 0] * rhs.X + matr[0, 1] * rhs.Y, matr[1, 0] * rhs.X + matr[1, 1] * rhs.Y);
        }
    }
}