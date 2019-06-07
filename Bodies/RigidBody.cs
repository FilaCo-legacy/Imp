using ImpLite.BroadPhase;

namespace ImpLite.Bodies
{
    /// <summary>
    /// Represents some body's model in the physics engine
    /// </summary>
    public partial class RigidBody : IBody, IMask, IBoxable
    {
        /// <summary>
        /// Coords of the material point
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Sum of force-vectors applied to this body
        /// </summary>
        public Vector2 Force { get; set; }
        
        public Vector2 LinearVelocity { get; set; }
        
        /// <summary>
        /// Info about this body's material
        /// </summary>
        public RigidMaterial Material { get; set; }

        public float Mass
        {
            get
            {
                if (InverseMass < ImpParams.GetInstance.Epsilon)
                    return 0.0f;

                return 1.0f / InverseMass;
            }
        }
        
        public float InverseMass { get; set; }

        /// <summary>
        /// Defines if external force might be applied to this body
        /// </summary>
        public bool IsKinematic { get; set; }

        public void SetStatic()
        {
            InverseMass = 0.0f;
        }

        public void ApplyForceToCenter(Vector2 force)
        {
            Force += force;
        }
        
        public void ApplyImpulseToCenter(Vector2 impulse)
        {
            LinearVelocity += InverseMass * impulse;
        }
    }
}