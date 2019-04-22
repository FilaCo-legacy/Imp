using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Структура представляющая матрицу поворота 2х2
    /// </summary>
    public struct TMat22
    {
        private float[,] matr;
        /// <summary>
        /// Элементы матрицы
        /// </summary>
        public float[,] Matr => matr;
        /// <summary>
        /// Вектор из значений первой колонки
        /// </summary>
        public Vector GetColumnX => new Vector(matr[0, 0], matr[0, 1]);
        /// <summary>
        /// Вектор из значений второй колонки
        /// </summary>
        public Vector GetColumnY => new Vector(matr[1, 0], matr[1, 1]);
        public float this [int i, int j] { get => matr[i, j]; set => matr[i, j] = value; }
        /// <summary>
        /// Инициализирует новый объект <see cref="TMat22"/> из двух векторов
        /// </summary>
        /// <param name="valueX"></param>
        /// <param name="valueY"></param>
        public TMat22(Vector valueX, Vector valueY)
        {
            matr = new float[2, 2];
            matr[0, 0] = valueX.X;
            matr[0, 1] = valueX.Y;
            matr[1, 0] = valueY.X;
            matr[1, 1] = valueY.Y;
        }
        /// <summary>
        /// Инициализирует новый объект <see cref="TMat22"/> из данного угла
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public TMat22(float angleRad)
        {
            matr = new float[2, 2];
            Set(angleRad);
        }
        /// <summary>
        /// Инициализирует новый объект <see cref="TMat22"/> из четырёх чисел
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public TMat22(float m00, float m01, float m10, float m11)
        {
            matr = new float[2, 2];
            matr[0, 0] = m00;
            matr[0, 1] = m01;
            matr[1, 0] = m10;
            matr[1, 1] = m11;
        }
        /// <summary>
        /// Инициализирует новый объект <see cref="TMat22"/> из другой матрицы поворота
        /// </summary>
        /// <param name="pref"></param>
        public TMat22(TMat22 pref)
        {
            matr = new float[2, 2];
            for (int i = 0; i < 2; ++i)
                for (int j = 0; j < 2; ++j)
                    matr[i, j] = pref[i, j];
        }
        /// <summary>
        /// Настраивает матрицу поворота на заданный угол
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public void Set(float angleRad)
        {
            float angleCos = (float)Math.Cos(angleRad);
            float angleSin = (float)Math.Sin(angleRad);
            matr[0, 0] = angleCos;
            matr[0, 1] = -angleSin;
            matr[1, 0] = angleSin;
            matr[1, 1] = angleCos;
        }
        /// <summary>
        /// Транспонирование матрицы поворота
        /// </summary>
        /// <returns></returns>
        public TMat22 Transpose()
        {
            return new TMat22(matr[0, 0], matr[1, 0], matr[0, 1], matr[1, 1]);
        }
        /// <summary>
        /// Поворот вектора
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector operator * (TMat22 matr, Vector rhs)
        {
            return new Vector(matr[0, 0] * rhs.X + matr[0, 1] * rhs.Y, matr[1, 0] * rhs.X + matr[1, 1] * rhs.Y);
        }
    }
}
