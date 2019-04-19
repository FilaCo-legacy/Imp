namespace PhysEngine
{
    /// <summary>
    /// Интерфейс, представляющий информацию о материале в физическом движке
    /// </summary>
    public interface IMaterial
    {
        string Name { get; set; }
        float Density { get; }
        float Restitution { get; }
    }
}
