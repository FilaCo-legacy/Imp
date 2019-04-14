using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public enum TypeMaterial { ROCK, WOOD, METAL, BOUNCY_BALL, SUPER_BALL, PILLOW, STATIC};
    /// <summary>
    /// Структура, хранящая информацию о материале объекта
    /// </summary>
    public struct TMaterial
    {
        private float density;
        private float restitution;
        private static float[] densities = new float[] { 0.6f, 0.3f, 1.2f, 0.3f, 0.3f, 0.1f, 0.0f };
        private static float[] restitutions = new float [] { 0.1f, 0.2f, 0.05f, 0.8f, 0.95f, 0.2f, 0.4f};
        /// <summary>
        /// Плотность объекта
        /// </summary>
        public float Density => density;
        /// <summary>
        /// Коэффициент упругости объекта
        /// </summary>
        public float Restitution => restitution;
        public TMaterial(TypeMaterial typeMaterial)
        {
            density = densities[(int)typeMaterial];
            restitution = restitutions[(int)typeMaterial];
        }
    }
}
