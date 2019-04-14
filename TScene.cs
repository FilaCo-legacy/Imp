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
        /// <summary>
        /// Общая точность вычислений для всех операций над объектами
        /// </summary>
        public const float EPS = 1e-4F;
        public const float DEFAULT_DELTA_TIME = 1.0f / 60.0f;
        public const float DEFAULT_GRAVITY_SCALE = 5.0f;
        public static Vector2D DEFAULT_GRAVITY = new Vector2D(0, 9.8f * DEFAULT_GRAVITY_SCALE);
        private float dt;
        private uint iterations;
        private Vector2D gravity;
        private float gravityScale;
        private List<TBody> bodies = new List<TBody>();
        private List<TManifold> contacts = new List<TManifold>();
        /// <summary>
        /// Прирост времени модели сцены
        /// </summary>
        public float Dt => dt;
        /// <summary>
        /// Вектор силы гравитации сцены
        /// </summary>
        public Vector2D Gravity => gravity;
        public int ScaleVisual { get; set; }
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TScene"/>
        /// </summary>
        /// <param name="valueIterations"></param>
        /// <param name="valueScaleVisual">Масштаб для перевода координат на отображаемую плоскость</param>
        /// <param name="valueGravity">Вектор гравитации, действующий на все тела в сцене</param>
        /// <param name="valueDt">Значение "прироста" времени</param>
        /// <param name="valueGrScale">Множитель гравитации</param>
        public TScene(uint valueIterations, int valueScaleVisual, Vector2D valueGravity, float valueDt = DEFAULT_DELTA_TIME, 
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
        /// Интегрирование силы, воздействующей на объект <see cref="TBody"/>
        /// </summary>
        /// <param name="valueBody">Целевой объект</param>
        private void IntegrateForces(TBody valueBody)
        {
            if (valueBody.InverseMass == 0.0f)
                return;
            valueBody.Velocity += (valueBody.Force * valueBody.InverseMass + gravity) * (dt / 2.0f);
            valueBody.AngularVelocity += valueBody.Torque * valueBody.InverseInertia * (dt / 2.0f);
        }
        /// <summary>
        /// Интегрирование скорости объекта <see cref="TBody"/>
        /// </summary>
        /// <param name="valueBody">Целевой объект</param>
        private void IntegrateVelocity(TBody valueBody)
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
                TBody objectA = bodies[i];
                for (int j = i + 1; j < bodies.Count; ++j)
                {
                    TBody objectB = bodies[j];
                    if (objectA.InverseMass == 0 && objectB.InverseMass == 0)
                        continue;
                    TManifold curManifold = new TManifold(objectA, objectB, this);
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
                TBody curBody = bodies[i];
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
        public void Add(IShape valueShape, Vector2D valueCenter, TypeMaterial valueMaterial)
        {
            if (valueShape == null)
                throw new Exception("Форма была не назначена");
            TBody curBody = new TBody(valueShape, valueCenter, valueMaterial);
            bodies.Add(curBody);
            valueShape.Body = curBody;
        }
    }
}
