using System;
using System.Collections.Generic;
using System.Drawing;

namespace PhysEngine
{

    // Ускорение
    //    F = mA
    // => A = F * 1/m

    // Явный метод интегрирования Эйлера
    // x += v * dt
    // v += (1/m * F) * dt

    // Симплетический метод Эйлера
    // v += (1/m * F) * dt
    // x += v * dt
    /// <summary>
    /// Представляет собой сцену, на которой взаимодействуют физические модели
    /// </summary>
    public class TScene
    {
        private float dt;
        private uint iterations;
        private Vector gravity;
        private float gravityScale;
        private List<Body> bodies = new List<Body>();
        private List<Manifold> contacts = new List<Manifold>();
        /// <summary>
        /// Прирост времени модели сцены
        /// </summary>
        public float Dt => dt;
        /// <summary>
        /// Вектор силы гравитации сцены
        /// </summary>
        public Vector Gravity => gravity;
        public int ScaleVisual { get; set; }
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TScene"/>
        /// </summary>
        /// <param name="valueIterations"></param>
        /// <param name="valueScaleVisual">Масштаб для перевода координат на отображаемую плоскость</param>
        /// <param name="valueGravity">Вектор гравитации, действующий на все тела в сцене</param>
        /// <param name="valueDt">Значение "прироста" времени</param>
        /// <param name="valueGrScale">Множитель гравитации</param>
        public TScene(uint valueIterations, int valueScaleVisual, Vector valueGravity, float valueDt = DEFAULT_DELTA_TIME, 
            float valueGrScale = DEFAULT_GRAVITY_SCALE)
        {
            ScaleVisual = valueScaleVisual;
            gravity = valueGravity;
            gravityScale = valueGrScale;
            dt = valueDt;
            iterations = valueIterations;
        }
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TScene"/>
        /// Вектор гравитации задаётся автоматически как <see cref="DEFAULT_GRAVITY"/>
        /// </summary>
        /// <param name="valueIterations"></param>
        /// <param name="valueScaleVisual">Масштаб для перевода координат на отображаемую плоскость</param>
        /// <param name="valueDt">Значение "прироста" времени</param>
        /// <param name="valueGrScale">Множитель гравитации</param>
        public TScene(uint valueIterations, int valueScaleVisual, float valueDt = DEFAULT_DELTA_TIME,
            float valueGrScale = DEFAULT_GRAVITY_SCALE)
        {
            ScaleVisual = valueScaleVisual;
            gravity = DEFAULT_GRAVITY;
            gravityScale = valueGrScale;
            dt = valueDt;
            iterations = valueIterations;
        }
        /// <summary>
        /// Интегрирование силы, воздействующей на объект <see cref="Body"/>
        /// </summary>
        /// <param name="valueBody">Целевой объект</param>
        private void IntegrateForces(Body valueBody)
        {
            if (valueBody.InverseMass == 0.0f)
                return;
            valueBody.Velocity += (valueBody.Force * valueBody.InverseMass + gravity) * (dt / 2.0f);
            valueBody.AngularVelocity += valueBody.Torque * valueBody.InverseInertia * (dt / 2.0f);
        }
        /// <summary>
        /// Интегрирование скорости объекта <see cref="Body"/>
        /// </summary>
        /// <param name="valueBody">Целевой объект</param>
        private void IntegrateVelocity(Body valueBody)
        {
            if (valueBody.InverseMass == 0.0f)
                return;
            IntegrateForces(valueBody);
            valueBody.Position += valueBody.Velocity * dt;
            valueBody.Orient += valueBody.AngularVelocity * dt;
            IntegrateForces(valueBody);
        }
        /// <summary>
        /// Поиск и разрешение коллизий между объектами на сцене, интегрирование скоростей объектов и сил, воздействующих на них
        /// </summary>
        public void Step()
        {
            // Собираем новую информацию о коллизиях
            contacts.Clear();
            for (int i = 0; i < bodies.Count; ++i)
            {
                Body objectA = bodies[i];
                for (int j = i + 1; j < bodies.Count; ++j)
                {
                    Body objectB = bodies[j];
                    if (objectA.InverseMass == 0 && objectB.InverseMass == 0)
                        continue;
                    Manifold curManifold = new Manifold(objectA, objectB, this);
                    curManifold.Solve();
                    if (curManifold.ContactNumber > 0)
                        contacts.Add(curManifold);
                }
            }

            // Инициализируем коллизии
            for (int i = 0; i < contacts.Count; ++i)
                contacts[i].Initialize();

            // Разрешаем коллизии
            for (int j = 0; j < iterations; ++j)
                for (int i = 0; i < contacts.Count; ++i)
                    contacts[i].ApplyImpulse();

            // Интегрируем скорости
            for (int i = 0; i < bodies.Count; ++i)
                IntegrateVelocity(bodies[i]);

            // Поправляем позиции
            for (int i = 0; i < contacts.Count; ++i)
                contacts[i].PositionalCorrection();

            // Убираем все силы
            for (int i = 0; i < bodies.Count; ++i)
            {
                Body curBody = bodies[i];
                curBody.Force.Set(0, 0);
                curBody.Torque = 0;
            }
        }
        /// <summary>
        /// Отрисовка всех тел, находящихся на сцене
        /// </summary>
        /// <param name="g"></param>
        public void Render(Graphics g)
        {
            foreach (var curBody in bodies)
            {
                curBody.GetShape.Draw(g, ScaleVisual);
            }
        }
        /// <summary>
        /// Добавление нового объекта на сцену
        /// </summary>
        /// <param name="valueShape">Форма нового объекта</param>
        /// <param name="valueCenter">Позиция, на которую добавляется объект</param>
        /// <returns></returns>
        public void Add(IShape valueShape, Vector valueCenter, TypeMaterial valueMaterial)
        {
            if (valueShape == null)
                throw new Exception("Форма была не назначена");
            Body curBody = new Body(valueShape, valueCenter, valueMaterial);
            bodies.Add(curBody);
            valueShape.Body = curBody;
        }
    }
}
