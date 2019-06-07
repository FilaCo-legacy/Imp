using System;
using ImpLite.Bodies;
using ImpLite.NarrowPhase.Solvers;

namespace ImpLite.NarrowPhase
{
    public class Collider : ICollider
    {
        private float _mixedRestitution;
        private float _mixedStaticFriction;
        private float _mixedDynamicFriction;
        
        public IBody BodyA { get; }
        
        public IBody BodyB { get; }

        public float Penetration { get; set; }
        
        public Vector2 Normal { get; set; }
        
        public Vector2 [] Contacts { get; }
        
        public int ContactNumber { get; set; }
        
        public ISolver Solver { get; set; }

        public Collider(IBody bodyA, IBody bodyB)
        {
            BodyA = bodyA;
            BodyB = bodyB;
            Contacts = new Vector2[2];
        }

        private void InfiniteMassCorrection()
        {
            BodyA.LinearVelocity.Set(0.0f, 0.0f);
            BodyB.LinearVelocity.Set(0.0f, 0.0f);
        }

        private Vector2 ComputeImpulse()
        {
            var relativeVelocity = BodyB.LinearVelocity - BodyA.LinearVelocity;

            var contactVelocity = Vector2.DotProduct(relativeVelocity, Normal);

            var invMassSum = BodyA.InverseMass + BodyB.InverseMass;

            var absImpulse = -contactVelocity;

            absImpulse /= invMassSum;

            absImpulse /= ContactNumber;

            return Normal * absImpulse;
        }

        private Vector2 ComputeFrictionImpulse(float absImpulse)
        {
            var relativeVelocity = BodyB.LinearVelocity - BodyA.LinearVelocity;   
            var invMassSum = BodyA.InverseMass + BodyB.InverseMass;
            
            var tangent = relativeVelocity - Normal * Vector2.DotProduct(Normal, relativeVelocity);
            tangent.Normalize();

            var absFrictionImpulse = -Vector2.DotProduct(relativeVelocity, tangent);
            absFrictionImpulse /= invMassSum;
            absFrictionImpulse /= ContactNumber;

            if (Math.Abs(absFrictionImpulse) < ImpParams.GetInstance.Epsilon)
                return new Vector2(0, 0);

            if (Math.Abs(absFrictionImpulse) < absImpulse * _mixedStaticFriction)
                return tangent * absFrictionImpulse;
            else
                return tangent * -absImpulse * _mixedDynamicFriction;
        }
        
        public void PositionalCorrection()
        {
            var percentage = ImpParams.GetInstance.PercentLinearProjection;
            var slop = ImpParams.GetInstance.Slop;
            
            var correction = Math.Max(Penetration - slop, 0.0f) / 
                                  (BodyA.InverseMass + BodyB.InverseMass) * percentage * Normal;
            BodyA.Position -= BodyA.InverseMass * correction;
            BodyB.Position += BodyB.InverseMass * correction;
        }

        public void Initialize()
        {
            Solver.ResolveCollision(this);
            
            _mixedRestitution= Math.Min(BodyA.Material.Restitution, BodyB.Material.Restitution);
            
            _mixedStaticFriction = (float)Math.Sqrt(BodyA.Material.StaticFriction * BodyB.Material.StaticFriction);
            _mixedDynamicFriction = (float)Math.Sqrt(BodyA.Material.DynamicFriction * BodyB.Material.DynamicFriction);

            for (var i = 0; i < ContactNumber; ++i)
            {
                var relativeVelocity = BodyB.LinearVelocity - BodyA.LinearVelocity;

                var timeStep = ImpParams.GetInstance.TimeStep;
                var gravity = ImpParams.GetInstance.Gravity;
                var eps = ImpParams.GetInstance.Epsilon;
                
                if (relativeVelocity.LengthSquared < (timeStep * gravity).LengthSquared + eps)
                    _mixedRestitution = 0.0f;
            }
        }

        public void ApplyImpulse()
        {
            if (BodyA.Mass + BodyB.Mass < ImpParams.GetInstance.Epsilon)
            {
                InfiniteMassCorrection();
                return;
            }

            for (var i = 0; i < ContactNumber; ++i)
            {
                var impulse = ComputeImpulse();
                
                BodyA.ApplyImpulseToCenter(-impulse);
                BodyB.ApplyImpulseToCenter(impulse);

                var frictionImpulse = ComputeFrictionImpulse(impulse.Length);
                
                BodyA.ApplyImpulseToCenter(-frictionImpulse);
                BodyB.ApplyImpulseToCenter(frictionImpulse);
            }
        }

        public bool Equals(ICollider other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(BodyA, other.BodyA) && Equals(BodyB, other.BodyB);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return obj.GetType() == this.GetType() && Equals((Collider) obj);
        }

        public override int GetHashCode()
        {
            var bodyAHash = BodyA.GetHashCode();
            var bodyBHash = BodyB.GetHashCode();

            var str = string.Format(bodyAHash.ToString() + bodyBHash.ToString());
            
            const int p = 31;

            var hash = 0;
            var pPow = 1;

            for (var i = 0; i < str.Length; ++i)
            {
                hash += (str[i] - '0' + 1) * pPow;

                pPow *= p;
            }

            return hash;
        }
    }
}