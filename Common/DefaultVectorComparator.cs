using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine.Common
{
    internal class DefaultVectorComparator : IComparer<Vector>
    {
        int IComparer<Vector>.Compare(Vector vA, Vector vB)
        {
            if (Math.Abs(vA.X - vB.X) < PhysEngineDefaults.EPS)
            {
                if (Math.Abs(vA.Y - vB.Y) < PhysEngineDefaults.EPS)
                    return 0;
                return vA.Y.CompareTo(vB.Y);
            }
            return vA.X.CompareTo(vB.X);
        }
    }
}
