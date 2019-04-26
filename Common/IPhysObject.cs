using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.Common
{
    public interface IPhysObject
    {
        ushort Mask { get; set; }

        ushort Category { get; set; }

        short GroupIndex { get; set; }
    }
}
