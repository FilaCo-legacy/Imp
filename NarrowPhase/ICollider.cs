using System;
using Imp.Bodies;

namespace Imp.NarrowPhase
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