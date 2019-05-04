using PhysEngine.Common;

namespace PhysEngine.CollisionDetection.BroadPhase
{
    /// <summary>
    /// Структура, представляющая собой прямоугольник, выровненный по осям координат (Axis-Aligned Bounding Box)
    /// </summary>
    public struct AABB
    {
        public Vector LeftUpper { get; }

        public Vector RightLower { get; }

        public float Width => RightLower.X - LeftUpper.X;

        public float Height => RightLower.Y - LeftUpper.Y;

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
