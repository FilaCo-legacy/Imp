using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Сборник констант для физического движка
    /// </summary>
    public static class PhysEngineConsts
    {
        /// <summary>
        /// Общая точность вычислений для всех операций с плавающей точкой
        /// </summary>
        public const float EPS = 1e-7f;

        /// <summary>
        /// Стандартный квант времени
        /// </summary>
        public const float DT = 1.0f / 60.0f;

        /// <summary>
        /// Ускорение свободного падения
        /// </summary>
        public const float G = 9.8f;
    }
}
