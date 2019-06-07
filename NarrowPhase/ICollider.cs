using System;
using ImpLite.Bodies;

namespace ImpLite.NarrowPhase
{
    public interface ICollider : IEquatable<ICollider>
    {
        IBody BodyA { get; }
        
        IBody BodyB { get; }

        void Initialize();

        void ApplyImpulse();

        void PositionalCorrection();
    }
}