using System;

namespace ImpLite
{
    /// <summary>
    /// Structure that implements a vector on the plane
    /// </summary>
    public struct Vector2
    {
        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float LengthSquared => X * X + Y * Y;
        
        public float Length => (float)Math.Sqrt(X * X + Y * Y);
        
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initialize new instance of <see cref="Vector2"/> as a copy of the given instance
        /// </summary>
        /// <param name="original">Origin vector</param>
        public Vector2(Vector2 original)
        {
            X = original.X;
            Y = original.Y;
        }

        #region Operators
        
        public static Vector2 operator -(Vector2 vector2)
        {
            return new Vector2(-vector2.X, -vector2.Y);
        }

        public static Vector2 operator -(Vector2 vA, Vector2 vB)
        {
            return new Vector2(vA.X - vB.X, vA.Y - vB.Y);
        }

        public static Vector2 operator -(Vector2 vector2, float value)
        {
            return new Vector2(vector2.X - value, vector2.Y - value);
        }

        public static Vector2 operator +(Vector2 vA, Vector2 vB)
        {
            return new Vector2(vA.X + vB.X, vA.Y + vB.Y);
        }

        public static Vector2 operator +(Vector2 vector2, float value)
        {
            return new Vector2(vector2.X + value, vector2.Y + value);
        }

        public static Vector2 operator /(Vector2 vec, float divider)
        {
            return new Vector2(vec.X / divider, vec.Y / divider);
        }

        public static Vector2 operator *(Vector2 vec, float multiplier)
        {
            return new Vector2(vec.X * multiplier, vec.Y * multiplier);
        }

        public static Vector2 operator *(float multiplier, Vector2 vec)
        {
            return new Vector2(vec.X * multiplier, vec.Y * multiplier);
        }

        public static bool operator ==(Vector2 vA, Vector2 vB)
        {
            if (Math.Abs(vA.X - vB.X) < ImpParams.GetInstance.Epsilon &&
                Math.Abs(vA.Y - vB.Y) < ImpParams.GetInstance.Epsilon)
                return true;

            return false;
        }

        public static bool operator !=(Vector2 vA, Vector2 vB)
        {
            return !(vA == vB);
        }
        #endregion
        
        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public static float CrossProduct(Vector2 vA, Vector2 vB)
        {
            return vA.X * vB.Y - vA.Y * vB.X;
        }
        
        public static Vector2 CrossProduct(Vector2 vec, float value)
        {
            return new Vector2(value * vec.Y, -value * vec.X);
        }
        
        public static Vector2 CrossProduct(float value, Vector2 vec)
        {
            return new Vector2(-value * vec.Y, value * vec.X);
        }
        
        public static float DotProduct(Vector2 vA, Vector2 vB)
        {
            return vA.X * vB.X + vA.Y * vB.Y;
        }
        
        public void Normalize()
        {
            var length = Length;

            if (length < ImpParams.GetInstance.Epsilon)
                return;

            var invLen = 1.0f / Length;
            X *= invLen;
            Y *= invLen;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2))
            {
                return false;
            }

            var vector = (Vector2)obj;
            
            return this == vector;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
