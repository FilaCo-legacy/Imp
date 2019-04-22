using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public class DefaultVectorComparator : IComparer<Vector>
    {
        int IComparer<Vector>.Compare(Vector a, Vector b)
        {
            if (Math.Abs(a.X - b.X) < PhysEngineConsts.EPS)
            {
                if (Math.Abs(a.Y - b.Y) < PhysEngineConsts.EPS)
                    return 0;
                return a.Y.CompareTo(b.Y);
            }
            return a.X.CompareTo(b.X);
        }
    }
}
