namespace ImpLite.Bodies
{
    /// <summary>
    /// Provides an interface to build some instance of <see cref="RigidBody"/>
    /// </summary>
    public interface IRigidBodyBuilder
    {
        void Reset();

        void SetMaterial(float density, float restitution, float staticFriction, float dynamicFriction);

        void SetPosition(Vector2 position);

        void SetMask(ushort maskBits, ushort categoryBits);

        void SetBox(float width, float height);

        void ComputeMass();

        void SetStatic(bool flag);

        void SetKinematic(bool flag);
    }
}