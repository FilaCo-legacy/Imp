using System;

namespace PhysEngine
{
    /// <summary>
    /// Представляет 2D вектор
    /// </summary>
    public struct Vector2D:IComparable<Vector2D>
    {
        public float X { get; set; }
        public float Y { get; set; }
        /// <summary>
        /// Длина вектора в квадрате
        /// </summary>
        public float LengthSquared => this * this;
        /// <summary>
        /// Длина вектора
        /// </summary>
        public float Length => (float)Math.Sqrt(LengthSquared);
        /// <summary>
        /// Инициализирует новый экземпляр структуры <see cref="Vector2D"/>
        /// </summary>
        /// <param name="valueX">Абсцисса конца вектора</param>
        /// <param name="valueY">Ордината конца вектора</param>
        public Vector2D(float valueX, float valueY)
        {
            X = valueX;
            Y = valueY;
        }
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Vector2D"/> как копию данного вектора
        /// </summary>
        /// <param name="original">Исходный вектор</param>
        public Vector2D(Vector2D original)
        {
            X = original.X;
            Y = original.Y;
        }
        #region Operators
        public static Vector2D operator - (Vector2D vector)
        {
            return new Vector2D(-vector.X, -vector.Y);
        }
        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }
        public static Vector2D operator -(Vector2D vector, float value)
        {
            return new Vector2D(vector.X - value, vector.Y - value);
        }
        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2D operator + (Vector2D vector, float value)
        {
            return new Vector2D(vector.X + value, vector.Y + value);
        }
        public static Vector2D operator /(Vector2D vec, float divider)
        {
            return new Vector2D(vec.X / divider, vec.Y / divider);
        }
        public static Vector2D operator *(Vector2D vec, float multiplier)
        {
            return new Vector2D(vec.X * multiplier, vec.Y * multiplier);
        }
        public static Vector2D operator *(float multiplier, Vector2D vec)
        {
            return new Vector2D(vec.X * multiplier, vec.Y * multiplier);
        }
        /// <summary>
        /// Векторное произведение двух <see cref="Vector2D"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float operator ^(Vector2D a, Vector2D b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
        /// <summary>
        /// Векторное произведение вектора на скаляр
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2D operator ^(Vector2D vec, float value)
        {
            return new Vector2D(value * vec.Y, -value * vec.X);
        }
        /// <summary>
        /// Векторное произведение скаляра на вектор
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2D operator ^(float value, Vector2D vec)
        {
            return new Vector2D(-value * vec.Y, value * vec.X);
        }
        /// <summary>
        /// Скалярное произведение двух <see cref="Vector2D"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float operator *(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        public static bool operator == (Vector2D a, Vector2D b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator != (Vector2D a, Vector2D b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        #endregion
        /// <summary>
        /// Перенастроить вектор на указанные параметры
        /// </summary>
        /// <param name="valueX"></param>
        /// <param name="valueY"></param>
        public void Set(float valueX, float valueY)
        {
            X = valueX;
            Y = valueY;
        }
        public int CompareTo(Vector2D other)
        {
            if (Math.Abs(X - other.X) < TScene.EPS)
            {
                if (Y > other.Y)
                    return 1;
                else if (Math.Abs(Y - other.Y) < TScene.EPS)
                    return 0;
                else
                    return -1;
            }
            return X.CompareTo(other.X);
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
    }
}
