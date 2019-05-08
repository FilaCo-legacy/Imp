using PhysEngine.Common;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    /// <summary>
    /// Axis-Aligned Bounding Box
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct AABB
    {
        /// <summary>
        /// Coordinates of a left-upper corner of the <see cref="AABB"/>
        /// </summary>
        public Vector LeftUpper { get; }

        /// <summary>
        /// Coordinates of a right-lower corner of the <see cref="AABB"/>
        /// </summary>
        public Vector RightLower { get; }
        
        public float Width => RightLower.X - LeftUpper.X;

        public float Height => RightLower.Y - LeftUpper.Y;

        /// <summary>
        /// Coordinates of a center point of the <see cref="AABB"/>
        /// </summary>
        public Vector Center => 0.5f * (LeftUpper + RightLower);

        public AABB(float x, float y, float width, float height)
        {
            LeftUpper = new Vector(x, y);
            RightLower = new Vector(x + width, y + height);
        }

        public AABB(Vector leftUpper, Vector rightLower)
        {
            LeftUpper = leftUpper;
            RightLower = rightLower;
        }

        /// <summary>
        /// Checks if the AABBs are collided or not
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreCollided(AABB a, AABB b)
        {
            if (a.RightLower.X < b.LeftUpper.X || a.LeftUpper.X > b.RightLower.X)            
                return false;
            
            if (a.RightLower.Y < b.LeftUpper.Y || b.LeftUpper.Y > b.RightLower.Y)
                return false;            

            return true;
        }
    }
}
