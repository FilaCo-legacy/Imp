using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public interface IVectorComponent
    {
        Vector Velocity { get; set; }
        Vector Force { get; set; }
        Vector Position { get; set; }
        void ApplyForce(Vector force);
    }
}
