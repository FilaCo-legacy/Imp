using System.Numerics;
using Imp.BroadPhase;

namespace Imp.Bodies
{
    public interface IBody : IMask, IBoxable
    {
        Vector2 LinearVelocity { get; set; }
        
        float InverseMass { get; }
        
        float Mass { get; }
        
        Vector2 Position { get; set; }
        
        RigidMaterial Material { get; }

        void ApplyImpulseToCenter(Vector2 impulse);
    }
}