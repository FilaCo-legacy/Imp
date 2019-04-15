using System;

namespace PhysEngine
{
    /// <summary>
    /// Представляет вектор на плоскости
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// Абсцисса вектора
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Ордината вектора
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// Длина вектора в квадрате
        /// </summary>
        public float LengthSquared => DotProduct(this, this);
        /// <summary>
        /// Длина вектора
        /// </summary>
        public float Length => (float)Math.Sqrt(LengthSquared);
        /// <summary>
        /// Инициализирует новый экземпляр структуры <see cref="Vector"/>
        /// </summary>
        /// <param name="x">Абсцисса вектора</param>
        /// <param name="y">Ордината вектора</param>
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Vector"/> как копию данного вектора
        /// </summary>
        /// <param name="original">Исходный вектор</param>
        public Vector(Vector original)
        {
            X = original.X;
            Y = original.Y;
        }
        #region Operators
        public static Vector operator - (Vector vector)
        {
            return new Vector(-vector.X, -vector.Y);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }
        public static Vector operator -(Vector vector, float value)
        {
            return new Vector(vector.X - value, vector.Y - value);
        }
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }
        public static Vector operator + (Vector vector, float value)
        {
            return new Vector(vector.X + value, vector.Y + value);
        }
        public static Vector operator /(Vector vec, float divider)
        {
            return new Vector(vec.X / divider, vec.Y / divider);
        }
        public static Vector operator *(Vector vec, float multiplier)
        {
            return new Vector(vec.X * multiplier, vec.Y * multiplier);
        }
        public static Vector operator *(float multiplier, Vector vec)
        {
            return new Vector(vec.X * multiplier, vec.Y * multiplier);
        }         
        public static bool operator == (Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator != (Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        #endregion
        /// <summary>
        /// Перенастроить экземпляр структуры <see cref="Vector"/> на заданные параметры
        /// </summary>
        /// <param name="x">Новая абсцисса вектора</param>
        /// <param name="y">Новая ордината вектора</param>
        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Векторное произведение двух экземпляров структуры <see cref="Vector"/>
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Длина вектора-результата</returns>
        public static float CrossProduct(Vector a, Vector b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
        /// <summary>
        /// Векторное произведение экземпляра структуры <see cref="Vector"/> на скалярную величину
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector CrossProduct(Vector vec, float value)
        {
            return new Vector(value * vec.Y, -value * vec.X);
        }
        /// <summary>
        /// Векторное произведение скалярной величины на экземпляр структуры <see cref="Vector"/>
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector CrossProduct(float value, Vector vec)
        {
            return new Vector(-value * vec.Y, value * vec.X);
        }
        /// <summary>
        /// Скалярное произведение двух экземпляров структуры <see cref="Vector"/>
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns></returns>
        public static float DotProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        /// <summary>
        /// Нормализует вектор
        /// </summary>
        public void Normalize()
        {
            float tmpLen = Length;
            if (tmpLen > TScene.EPS)
            {
                float invLen = 1.0f / tmpLen;
                X *= invLen;
                Y *= invLen;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }

            var vector = (Vector)obj;
            return X == vector.X &&
                   Y == vector.Y;
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
