using PhysEngine.CollisionDetection.BroadPhase;

namespace PhysEngine.Common
{
    public abstract class PhysObject: IBoxable
    {
        public IMask MaskInfo { get; }

        public AABB GetBox { get; }
    }
}
