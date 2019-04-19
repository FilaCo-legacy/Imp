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
        public IMaterialPoint MaterialPoint { get; set; }

        public float Torque;
        public  float orient;
        public float AngularVelocity { get; set; }

        private TMaterial material;

        private float staticFriction;
        private float dynamicFriction;
        public float GetStaticFriction => staticFriction;
        public float GetDynamicFriction => dynamicFriction;

        
        /// <summary>
        /// Угол ориентации в радианах
        /// </summary>
        public float Orient
        {
            get => orient;
            set
            {
                orient = value;
                shape.SetOrient(orient);
            }
        }
        public TMaterial GetMaterial => material;     
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
