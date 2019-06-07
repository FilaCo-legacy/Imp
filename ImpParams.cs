namespace ImpLite
{
    public delegate bool MaskFilter (IMask lhs, IMask rhs);

    public class ImpParams
    {
        private static readonly ImpParams Instance = new ImpParams();

        public static ImpParams GetInstance => Instance;

        private ImpParams()
        {
            TimeStep = 1.0f / 60.0f;
            Gravity = new Vector2(0, 9.8f);
            Epsilon = 1e-7f;
            GravityScale = 5.0f;
            SceneIterations = 10;
            PercentLinearProjection = 0.4f;
            Slop = 0.05f;
        }

        public float TimeStep { get; set; }
        
        public Vector2 Gravity { get; set; }
        
        public float Epsilon { get; set; }
        
        public float GravityScale { get; set; }
        
        public int SceneIterations { get; set; }
        
        public float PercentLinearProjection { get; set; }
        
        public float Slop { get; set; }

        public bool DefaultFilter(IMask lhs, IMask rhs)
        {
            return (lhs.MaskBits & rhs.CategoryBits) != 0 && (lhs.CategoryBits & rhs.MaskBits) != 0;
        }
    }
}