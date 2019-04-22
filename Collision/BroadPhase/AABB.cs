namespace PhysEngine.Collision.BroadPhase
{
    /// <summary>
    /// Структура, представляющая собой прямоугольник, выровненный по осям координат (Axis-Aligned Bounding Box)
    /// </summary>
    public struct AABB
    {
        private Vector _leftUpper;
        private Vector _rightLower;

        public float Width => RightLower.X - LeftUpper.X;

        public float Height => RightLower.Y - LeftUpper.Y;

        public Vector LeftUpper
        {
            get => _leftUpper;
            set
            {
                var tmpWidth = Width;
                var tmpHeight = Height;
                _leftUpper = value;
                _rightLower.Set(LeftUpper.X + Width, LeftUpper.Y + Height);
            }
        }

        public Vector RightLower => _rightLower;

        public Vector Center => new Vector(LeftUpper.X + Width / 2, LeftUpper.Y + Height / 2);

        public AABB(float x, float y, float width, float height)
        {
            _leftUpper = new Vector(x, y);
            _rightLower = new Vector(x + width, y + height);
        }
    }
}
