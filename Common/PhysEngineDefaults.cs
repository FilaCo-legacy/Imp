using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.Common
{
    /// <summary>
    /// Сборник констант для физического движка
    /// </summary>
    public static class PhysEngineDefaults
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

        public static bool ShouldCollide(IPhysObject physObjA, IPhysObject physObjB)
        {
            // Если индекс группы хотя бы одного из объектов равна 0 или индексы групп не равны
            if (physObjA.GroupIndex == 0 || physObjB.GroupIndex == 0 || Math.Abs(physObjA.GroupIndex) !=
                Math.Abs(physObjB.GroupIndex))
            {
                return (physObjA.Mask & physObjB.Category) != 0 && (physObjB.Mask & physObjA.Category) != 0;
            }

            // Если знаки положительные, то объекты сталкиваются, иначе нет
            return Math.Sign(physObjA.GroupIndex) == 1;
        }
    }
}
