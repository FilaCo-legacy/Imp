using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysEngine.CollisionDetection.BroadPhase;
using PhysEngine.CollisionDetection.NarrowPhase;
using PhysEngine.Common;

namespace PhysEngine
{
    public class Scene
    {
        private BroadPhaseManager<PhysObject> _bpManager;

        private NarrowPhaseManager _npManager;

        public float TimeStep { get; set; }
        
        public List<PhysObject> Objects { get; }

        public Vector Gravity { get; }

        public MaskFilter Filter { get; set; }

        public void Step()
        {
            _bpManager.Initialize(Objects);

            for (var i = 0; i < Objects.Count; ++i)
            {

            }
        }
    }
}
