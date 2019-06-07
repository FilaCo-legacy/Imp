namespace ImpLite
{
    /// <summary>
    /// Interface that describes a category which the object is related and defines categories that might collide with it
    /// </summary>
    public interface IMask
    {
        /// <summary>
        /// Defines categories of objects that might collide with this object
        /// </summary>
        ushort MaskBits { get; set; }
        
        /// <summary>
        /// Describes a category of this object
        /// </summary>
        ushort CategoryBits { get; set; }
    }
}