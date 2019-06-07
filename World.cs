using System.Collections.Generic;
using ImpLite.Bodies;
using ImpLite.BroadPhase;
using ImpLite.NarrowPhase;

namespace ImpLite
{
    public class World
    {
        private readonly List<RigidBody> _bodies;
        private readonly IBroadPhaseManager<RigidBody> _bpManager;
        private readonly NarrowPhaseManager<Collider> _npManager;

        public MaskFilter Filter { get; set; }

        public int Width { get; set; }
        
        public int Height { get; set; }

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            Filter = ImpParams.GetInstance.DefaultFilter;
            
            _bodies = new List<RigidBody>();
            _bpManager = new BroadPhaseManager<RigidBody>(Width, Height);
            _npManager = new NarrowPhaseManager<Collider>();
        }

        private static void IntegrateForces(RigidBody body)
        {
            if (body.IsKinematic || body.InverseMass < ImpParams.GetInstance.Epsilon)
                return;

            var gravity = ImpParams.GetInstance.Gravity;
            var timeStep = ImpParams.GetInstance.TimeStep;
            
            body.LinearVelocity += (body.Force * body.InverseMass + gravity) * (timeStep / 2.0f);
        }

        private static void IntegrateVelocities(RigidBody body)
        {
            if (body.InverseMass < ImpParams.GetInstance.Epsilon)
                return;
            
            var timeStep = ImpParams.GetInstance.TimeStep;
            
            IntegrateForces(body);

            body.Position = body.LinearVelocity * timeStep;
            
            IntegrateForces(body);
        }

        private void ClearOutBoundsBodies()
        {
            _bodies.RemoveAll(body => body.Position.X < 0 || body.Position.Y < 0 || body.Position.X > Width ||
                                      body.Position.Y > Height);
        }
        
        private void ClearManagers()
        {
            _bpManager.Clear();
            _npManager.Clear();
        }

        private void ClearForces()
        {
            foreach (var cur in  _bodies)
            {
                cur.Force.Set(0.0f,0.0f);
            }
        }

        private void TransferPhase()
        {
            foreach (var lhsBody in _bodies)
            {
                var candidates = _bpManager.GetNeighbours(lhsBody);

                foreach (var rhsBody in candidates)
                {
                    if (Filter.Invoke(lhsBody, rhsBody))
                        _npManager.Add(lhsBody, rhsBody);
                }
            }
        }
        
        public void Step()
        {
            _bpManager.Execute(_bodies);

            TransferPhase();
            
            _npManager.Execute();

            foreach (var cur in _bodies)
            {
                IntegrateVelocities(cur);
            }
            
            _npManager.PositionalCorrection();
            
            ClearForces();
            
            ClearManagers();
            
            ClearOutBoundsBodies();
        }

        public void AddBody(RigidBody body)
        {
            _bodies.Add(body);
        }

        public void RemoveBody(RigidBody body)
        {
            _bodies.Remove(body);
        }
    }
}