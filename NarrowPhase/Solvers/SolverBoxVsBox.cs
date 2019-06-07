using System;
using ImpLite.Bodies;
using ImpLite.BroadPhase;

namespace ImpLite.NarrowPhase.Solvers
{
    /// <summary>
    /// An implementation of method that solves collision between two body shaped by<see cref="Box"/>
    /// </summary>
    internal class SolverBoxVsBox:ISolver
    {
        private IBody _bodyA;
        private IBody _bodyB;
        
        private Vector2 _normal;
        
        private Box _boxA;
        private Box _boxB;
        
        private void Initialize(ICollider collider)
        {
            _bodyA = collider.BodyA;
            _bodyB = collider.BodyB;

            _normal = _bodyB.Position - _bodyA.Position;

            _boxA = _bodyA.GetBox;
            _boxB = _bodyB.GetBox;
        }

        private float ComputeOverlapX()
        {
            var halfWidthA = _boxA.Width / 2.0f;
            var halfWidthB = _boxB.Width / 2.0f;

            return halfWidthA + halfWidthB - Math.Abs(_normal.X);
        }
        
        private float ComputeOverlapY()
        {
            var halfHeightA = _boxA.Height / 2.0f;
            var halfHeightB = _boxB.Height / 2.0f;

            return halfHeightA + halfHeightB - Math.Abs(_normal.Y);
        }

        public void ResolveCollision(Collider collider)
        {
            Initialize(collider);

            var xOverlap = ComputeOverlapX();
            var yOverlap = ComputeOverlapY();

            if (!(xOverlap > 0) || !(yOverlap > 0)) return;
            
            if (xOverlap < yOverlap)
            {
                collider.Normal = _normal.X < 0 ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);
            }
            else
            {
                collider.Normal = _normal.Y < 0 ? new Vector2(0.0f, -1.0f) : new Vector2(0.0f, 1.0f);
            }

            collider.Penetration = Math.Min(xOverlap, yOverlap);
        }
    }
}