using System;

namespace PhysEngine.Common
{
    /// <summary>
    /// Структура представляющая матрицу поворота 2х2
    /// </summary>
    public struct Mat22
    {
        /// <summary>
        /// Элементы матрицы
        /// </summary>
        public float[,] Matr { get; }

        /// <summary>
        /// Вектор из значений первого столбца
        /// </summary>
        public Vector GetColumnX => new Vector(Matr[0, 0], Matr[0, 1]);

        /// <summary>
        /// Вектор из значений второго столбца
        /// </summary>
        public Vector GetColumnY => new Vector(Matr[1, 0], Matr[1, 1]);

        public float this [int i, int j] { get => Matr[i, j]; set => Matr[i, j] = value; }

        /// <summary>
        /// Инициализирует новый объект <see cref="Mat22"/> из двух векторов
        /// </summary>
        /// <param name="valueX"></param>
        /// <param name="valueY"></param>
        public Mat22(Vector valueX, Vector valueY)
        {
            Matr = new float[2, 2];
            Matr[0, 0] = valueX.X;
            Matr[0, 1] = valueX.Y;
            Matr[1, 0] = valueY.X;
            Matr[1, 1] = valueY.Y;
        }

        /// <summary>
        /// Инициализирует новый объект <see cref="Mat22"/> из данного угла
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public Mat22(float angleRad)
        {
            Matr = new float[2, 2];
            Set(angleRad);
        }

        /// <summary>
        /// Инициализирует новый объект <see cref="Mat22"/> из четырёх чисел
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public Mat22(float m00, float m01, float m10, float m11)
        {
            Matr = new float[2, 2];
            Matr[0, 0] = m00;
            Matr[0, 1] = m01;
            Matr[1, 0] = m10;
            Matr[1, 1] = m11;
        }

        /// <summary>
        /// Инициализирует новый объект <see cref="Mat22"/> из другой матрицы поворота
        /// </summary>
        /// <param name="pref"></param>
        public Mat22(Mat22 pref)
        {
            Matr = new float[2, 2];
            for (int i = 0; i < 2; ++i)
                for (int j = 0; j < 2; ++j)
                    Matr[i, j] = pref[i, j];
        }

        /// <summary>
        /// Настраивает матрицу поворота на заданный угол
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public void Set(float angleRad)
        {
            var cosAngle = (float)Math.Cos(angleRad);
            var sinAngle = (float)Math.Sin(angleRad);
            Matr[0, 0] = cosAngle;
            Matr[0, 1] = -sinAngle;
            Matr[1, 0] = sinAngle;
            Matr[1, 1] = cosAngle;
        }

        /// <summary>
        /// Транспонирование матрицы поворота
        /// </summary>
        /// <returns></returns>
        public Mat22 Transpose()
        {
            return new Mat22(Matr[0, 0], Matr[1, 0], Matr[0, 1], Matr[1, 1]);
        }

        /// <summary>
        /// Поворот вектора
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector operator * (Mat22 matr, Vector rhs)
        {
            return new Vector(matr[0, 0] * rhs.X + matr[0, 1] * rhs.Y, matr[1, 0] * rhs.X + matr[1, 1] * rhs.Y);
        }
    }
}
