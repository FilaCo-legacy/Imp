namespace PhysEngine.CollisionDetection.BroadPhase
{
    public interface IBoxable
    {
        AABB GetBox { get; }
    }
}
