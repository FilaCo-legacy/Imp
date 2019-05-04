using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.Common
{
    class Body : IPhysObject, IBoxable
    {
        public AABB GetBox { get; }

        public ushort Mask { get; set; }
        public ushort Category { get; set; }
        public short GroupIndex { get; set; }
    }
}
