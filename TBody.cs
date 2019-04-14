using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет модель объекта в физическом движке
    /// </summary>
    public class TBody
    {
        #region TorqueData
        public float Torque;
        public  float orient;
        public float AngularVelocity { get; set; }
        #endregion

        private IShape shape;
        private TMaterial material;

        #region FrictionData
        private float staticFriction;
        private float dynamicFriction;
        public float GetStaticFriction => staticFriction;
        public float GetDynamicFriction => dynamicFriction;
        #endregion

        #region MassData
        private float inverseMass;
        private float inverseInertia;
        public float Mass
        {
            get
            {
                if (inverseMass == 0.0f)
                    return 0.0f;
                return 1.0f / inverseMass;
            }
            set
            {
                if (value == 0.0f)
                    inverseMass = 0.0f;
                else
                    inverseMass = 1.0f / value;
            }
        }
        public float InverseMass => inverseMass;
        public float Inertia
        {
            get
            {
                if (InverseInertia == 0.0f)
                    return 0.0f;
                return 1.0f / inverseInertia;
            }
            set
            {
                if (value == 0.0f)
                    inverseInertia = 0.0f;
                else
                    inverseInertia = 1.0f / value;
            }
        }
        public float InverseInertia => inverseInertia;
        #endregion
        /// <summary>
        /// Угол ориентации в радианах
        /// </summary>
        public float Orient
        {
            get => orient;
            set
            {
                orient = value;
                shape.SetOrient(orient);
            }
        }
        public TMaterial GetMaterial => material;
        public Vector2D Velocity { get; set; }
        public Vector2D Position { get; set; }
        public Vector2D Force { get; set; }        
        public IShape GetShape => shape;
        /// <summary>
        /// Инициализирует объект класса <see cref="TBody"/>
        /// </summary>
        /// <param name="valueShape">Форма создаваемого тела</param>
        /// <param name="valuePosition">Позиция, на которую помещается тело</param>
        /// <param name="valueMaterial">Тип материала создаваемого тела</param>
        public TBody(IShape valueShape, Vector2D valuePosition, TypeMaterial valueMaterial)
        {
            shape = (IShape)valueShape.Clone();
            shape.Body = this;
            material = new TMaterial(valueMaterial);
            Position = new Vector2D(valuePosition);
            Velocity = new Vector2D(0.0f, 0.0f);
            AngularVelocity = 0.0f;
            Torque = 0.0f;
            orient = 0.0f;
            Force = new Vector2D(0.0f, 0.0f);
            staticFriction = 0.5f;
            dynamicFriction = 0.3f;
            shape.Initialize();
        }
        /// <summary>
        /// Приложить к телу вектор силы
        /// </summary>
        /// <param name="valueForce"></param>
        public void ApplyForce(Vector2D valueForce)
        {
            Force += valueForce;
        }
        /// <summary>
        /// Приложить к телу импульс, задаваемый двумя <see cref="Vector2D"/>
        /// </summary>
        /// <param name="valueImpulse"></param>
        /// <param name="contactVector"></param>
        public void ApplyImpulse(Vector2D valueImpulse, Vector2D contactVector)
        {
            Velocity += inverseMass * valueImpulse;
            AngularVelocity += inverseInertia * (contactVector ^ valueImpulse);
        }
    }
}
