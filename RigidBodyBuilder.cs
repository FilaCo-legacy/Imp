using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    class RigidBodyBuilder : IBodyBuilder
    {
        private Body _body;

        public Body Result => _body;

        public void Reset()
        {
            _body = new RigidBody();
        }

        public void SetMaterial(IMaterial material)
        {
            _body.Material = material;
        }

        public void SetMaterialPoint()
        {
            throw new NotImplementedException();
        }

        public void SetShape()
        {
            throw new NotImplementedException();
        }
    }
}
