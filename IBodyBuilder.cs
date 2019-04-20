using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    interface IBodyBuilder
    {
        Body Result { get; }
        void Reset();
        void SetMaterial(IMaterial material);
        void SetMaterialPoint(IMaterialPoint point);
        void SetShape(IShape shape);        
    }
}
