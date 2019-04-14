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
    public abstract class Body
    {
        #region TorqueData
        public float Torque { get; set; }
        protected float _orient;
        public float AngularVelocity { get; set; }
        #endregion

        protected IShape _shape;
        protected IMaterial material;

        #region FrictionData
        private float staticFriction;
        private float dynamicFriction;
        public float GetStaticFriction => staticFriction;
        public float GetDynamicFriction => dynamicFriction;
        #endregion

        #region MassData
        private float _inverseMass;
        private float _inverseInertia;
        public float Mass
        {
            get
            {
                if (_inverseMass == 0.0f)
                    return 0.0f;
                return 1.0f / _inverseMass;
            }
        }
        public float InverseMass => _inverseMass;
        public float Inertia
        {
            get
            {
                if (InverseInertia == 0.0f)
                    return 0.0f;
                return 1.0f / _inverseInertia;
            }
        }
        public float InverseInertia => _inverseInertia;
        #endregion

        public Vector Velocity { get; set; }
        public Vector Position { get; set; }
        public Vector Force { get; set; }

        /// <summary>
        /// Угол ориентации в радианах
        /// </summary>
        public float Orient
        {
            get => _orient;
            set
            {
                _orient = value;
                _shape.SetOrient(_orient);
            }
        }
        public IMaterial GetMaterial => material;
     
        public IShape GetShape => _shape;
        /// <summary>
        /// Инициализирует объект класса <see cref="Body"/>
        /// </summary>
        /// <param name="shape">Форма создаваемого тела</param>
        /// <param name="position">Позиция, на которую помещается тело</param>
        /// <param name="valueMaterial">Тип материала создаваемого тела</param>
        public Body(IShape shape, Vector position)
        {
            this._shape = (IShape)shape.Clone();
            this._shape.Body = this;
            material = new IMaterial(valueMaterial);
            Position = new Vector(position);
            Velocity = new Vector(0.0f, 0.0f);
            AngularVelocity = 0.0f;
            Torque = 0.0f;
            Orient = 0.0f;
            Force = new Vector(0.0f, 0.0f);
            staticFriction = 0.5f;
            dynamicFriction = 0.3f;
            this._shape.Initialize();
        }
        /// <summary>
        /// Применить к телу вектор силы
        /// </summary>
        /// <param name="force"></param>
        public void ApplyForce(Vector force)
        {
            Force += force;
        }
        /// <summary>
        /// Применить к телу импульс, задавваемый вектором самого импульса и точкой приложения
        /// </summary>
        /// <param name="impulse">Вектор импульса</param>
        /// <param name="contactPpint">Точка приложения</param>
        public void ApplyImpulse(Vector impulse, Vector contactPoint)
        {
            Velocity += _inverseMass * impulse;
            AngularVelocity += _inverseInertia * Vector.CrossProduct(contactPoint,impulse);
        }
    }
}
