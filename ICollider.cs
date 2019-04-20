namespace PhysEngine
{
    interface ICollider
    {
        IBody First { get; }
        IBody Second { get; }
        bool AreCollided();
    }
}
