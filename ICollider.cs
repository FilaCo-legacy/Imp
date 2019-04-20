using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет собой коллайдер, вызывающий проверку столкновения двух объектов <see cref="Body"/>
    /// </summary>
    public interface ICollider
    {
        Body FirstBody { get; }
        Body SecondBody { get; }
        /// <summary>
        /// Проверяет, есть коллизия между двумя телами в коллайдере
        /// </summary>
        bool AreCollided();
    }
}
