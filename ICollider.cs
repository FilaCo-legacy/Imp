using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет структуру простого многообразия, хранящего информацию о коллизии двух объектов <see cref="Body"/>
    /// </summary>
    public interface ICollider
    {
        Body FirstBody { get; }
        Body SecondBody { get; }
        /// <summary>
        /// Проверяет, столкнулись ли 
        /// </summary>
        bool AreCollided();
        /// <summary>
        /// Инициализирует 
        /// </summary>
        void Initialize();
        /// <summary>
        /// Прикладывает импульс к системе из двух тел
        /// </summary>
        void ApplyImpulse();
    }
}
