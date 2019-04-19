using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет абстрактный объект в физическом движке
    /// </summary>
    public abstract class Body
    {
        public IMaterialPoint MaterialPoint { get; }
        public IMaterial Material { get; }
        public float Torque { get; set; }
        public float Orient { get; set; }
        public float AngularVelocity { get; set; }  
        /// <summary>
        /// Приложить к телу импульс, задаваемый двумя <see cref="Vector"/>
        /// </summary>
        /// <param name="impulse"></param>
        /// <param name="contactVector"></param>
        public void ApplyImpulse(Vector impulse, Vector contactVector)
        {
            MaterialPoint.VectorComponent.Velocity += MaterialPoint.InverseMass * impulse;
            AngularVelocity += MaterialPoint.InverseInertia * Vector.CrossProduct(contactVector, impulse);
        }
    }
}
