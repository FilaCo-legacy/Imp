using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет интерфейс доступа к объекту в физическом движке
    /// </summary>
    public interface IBody
    {
        IMaterialPoint MaterialPoint { get; }

        IMaterial Material { get; }

        IShape Shape { get; }
    }
}
