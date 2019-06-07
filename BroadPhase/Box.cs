namespace ImpLite.BroadPhase
{
    /// <summary>
    /// Axis-Aligned Bounding Box
    /// </summary>
    public struct Box
    {
        /// <summary>
        /// Coordinates of a left-upper corner of the <see cref="Box"/>
        /// </summary>
        public Vector2 LeftUpper { get; }

        /// <summary>
        /// Coordinates of a right-lower corner of the <see cref="Box"/>
        /// </summary>
        public Vector2 RightLower { get; }
        
        public float Width => RightLower.X - LeftUpper.X;

        public float Height => RightLower.Y - LeftUpper.Y;

        /// <summary>
        /// Coordinates of a center point of the <see cref="Box"/>
        /// </summary>
        public Vector2 Center => 0.5f * (LeftUpper + RightLower);

        public Box(float x, float y, float width, float height)
        {
            LeftUpper = new Vector2(x, y);
            RightLower = new Vector2(x + width, y + height);
        }

        public Box(Vector2 leftUpper, Vector2 rightLower)
        {
            LeftUpper = leftUpper;
            RightLower = rightLower;
        }

        /// <summary>
        /// Checks if this box intersects other box
        /// </summary>
        /// <returns></returns>
        public bool Intersects(Box other)
        {
            if (RightLower.X < other.LeftUpper.X || LeftUpper.X > other.RightLower.X)            
                return false;
            
            if (RightLower.Y < other.LeftUpper.Y || LeftUpper.Y > other.RightLower.Y)
                return false;            

            return true;
        }
    }
}
