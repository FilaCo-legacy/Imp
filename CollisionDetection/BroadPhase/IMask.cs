using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    public interface IMask
    {
        ushort Mask { get; set; }

        ushort Category { get; set; }

        short GroupIndex { get; set; }
    }
}
