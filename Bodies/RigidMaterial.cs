namespace ImpLite.Bodies
{
    /// <summary>
    /// Represents properties of some rigid material 
    /// </summary>
    public struct RigidMaterial
    {
        public float Density { get; }
         
        public float Restitution { get; }
        
        /// <summary>
        /// A coefficient of the friction when some other object moves on this object surface
        /// </summary>
        public float DynamicFriction { get; }
        
        /// <summary>
        /// A coefficient of the friction when some object doesn't move on this object surface
        /// </summary>
        public float StaticFriction { get; }

        public RigidMaterial(float density, float restitution, float dynamicFriction, float staticFriction)
        {
            Density = density;
            Restitution = restitution;
            DynamicFriction = dynamicFriction;
            StaticFriction = staticFriction;
        }
    }
}