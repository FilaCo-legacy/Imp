using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysEngine.CollisionDetection.BroadPhase;
using PhysEngine.CollisionDetection.NarrowPhase;

namespace PhysEngine
{
    public class Scene
    {
        private BroadPhaseManager _bpManager;

        private NarrowPhaseManager _npManager;

        public float TimeStep { get; set; }
        
        public List<IPhysObject> PhysObjects { get; }

        public Vector Gravity { get; }

        public MaskFilter Filter { get; set; }

        public void Step()
        {
            _bpManager.Initialize(PhysObjects);
        }

        public IPhysObject AddObject()
        {

        }

        public void RemoveObject(IPhysObject physObject)
        {

        }
    }
}
