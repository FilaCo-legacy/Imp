using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    struct RigidMaterial : IMaterial
    {
        private float _density;
        private float _restitution;

        public string Name { get; set; }

        public float Density => _density;

        public float Restitution => _restitution;

        public RigidMaterial(string name, float density, float restitution)
        {
            Name = name;
            _density = density;
            _restitution = restitution;
        }
    }
}
