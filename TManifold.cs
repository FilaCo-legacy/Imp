using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    /// <summary>
    /// Представляет структуру простого многообразия, хранящего информацию о коллизии двух объектов <see cref="TBody"/>
    /// </summary>
    public class TManifold
    {
        /// <summary>
        /// Процент линейного проецирования
        /// </summary>
        private const float PERCENTAGE = 0.4f;
        /// <summary>
        /// Процент "допуска" проникновения
        /// </summary>
        private const float SLOP = 0.05f;
        private TScene parentScene;
        private TBody objectA;
        private TBody objectB;
        private float mixedRestitution;
        private float mixedDynamicFriction;
        private float mixedStaticFriction;
        private Vector2D[] contacts = new Vector2D[2];
        /// <summary>
        /// Количество точек соприкосновения между телами
        /// </summary>
        public int ContactNumber { get; set; }
        /// <summary>
        /// Глубина проникновения одного тела в другое
        /// </summary>
        public float Penetration { get; set; }
        /// <summary>
        /// Вектор из А в B
        /// </summary>
        public Vector2D VectorAB { get; set; }
        /// <summary>
        /// Точки контакта двух тел
        /// </summary>
        public Vector2D[] Contacts => contacts;
        /// <summary>
        /// Инициализирует новый объект, храняющий информацию о контакте двух тел на сцене
        /// </summary>
        /// <param name="valueObjectA"></param>
        /// <param name="valueObjectB"></param>
        /// <param name="parent"></param>
        public TManifold(TBody valueObjectA, TBody valueObjectB, TScene parent)
        {
            objectA = valueObjectA;
            objectB = valueObjectB;
            parentScene = parent;
        }
        /// <summary>
        /// Вызывает разрешение коллизии для соответствующий объекту структуры <see cref="TManifold"/> тел
        /// </summary>
        public void Solve()
        {
            int typeShapeA = (int)objectA.GetShape.GetType;
            int typeShapeB = (int)objectB.GetShape.GetType;
            Collision.Dispatcher[typeShapeA, typeShapeB](this, objectA, objectB);
        }
        /// <summary>
        /// Функция, корректирующая позиции двух тел после разрешения коллизии
        /// </summary>
        public void PositionalCorrection()
        {            
            Vector2D correction = Math.Max(Penetration - SLOP, 0.0f) / 
                (objectA.InverseMass + objectB.InverseMass) * PERCENTAGE * VectorAB;
            objectA.Position -= objectA.InverseMass * correction;
            objectB.Position += objectB.InverseMass * correction;
        }        
        /// <summary>
        /// Инициализирует структуру <see cref="TManifold"/>
        /// </summary>
        /// <param name="parent"></param>
        public void Initialize()
        {
            // Рассчитаем общую упругость системы
            mixedRestitution= Math.Min(objectA.GetMaterial.Restitution, objectB.GetMaterial.Restitution);

            // Рассчитаем коэффициент силы трения скольжения и силы трения покоя системы
            mixedStaticFriction = (float)Math.Sqrt(objectA.GetStaticFriction * objectB.GetStaticFriction);
            mixedDynamicFriction = (float)Math.Sqrt(objectA.GetDynamicFriction * objectB.GetDynamicFriction);

            for (int i = 0; i < ContactNumber; ++i)
            {
                // Рассчитаем радиус от центра масс до точки контакта
                Vector2D radObjA = contacts[i] - objectA.Position;
                Vector2D radObjB = contacts[i] - objectB.Position;

                Vector2D relativeVelocity = objectB.Velocity + (objectB.AngularVelocity^radObjB) -
                          objectA.Velocity - (objectA.AngularVelocity^radObjA);


                // Определим, должны ли мы выполнить столкновение в состоянии покоя или нет
                // Идея в том, что если единственное, что движет этим объектом, это гравитация,
                // тогда столкновение должно быть выполнено без восстановления
                if (relativeVelocity.LengthSquared < (parentScene.Dt * parentScene.Gravity).LengthSquared + TScene.EPS)
                    mixedRestitution = 0.0f;
            }
        }
        /// <summary>
        /// Расчёт и прикладывание импульса двух тел, между которыми происходит коллизия
        /// </summary>
        public void ApplyImpulse()
        {
            // Если оба объекта имеют бесконечную массу
            if (objectA.InverseMass + objectB.InverseMass == 0.0f)
            {
                InfiniteMassCorrection();
                return;
            }

            for (int i = 0; i < ContactNumber; ++i)
            {
                // Рассчитаем радиус от центра масс до точки контакта
                Vector2D radObjA = contacts[i] - objectA.Position;
                Vector2D radObjB = contacts[i] - objectB.Position;

                Vector2D relativeVelocity = objectB.Velocity + (objectB.AngularVelocity ^ radObjB) -
                          objectA.Velocity - (objectA.AngularVelocity ^ radObjA);

                // Относительная скорость по направлению нормали
                float contactVel = relativeVelocity * VectorAB;

                // Не разрешаем коллизии, если скорости направлены в разную сторону
                if (contactVel > 0)
                    return;

                float radObjA_CrossN = radObjA ^ VectorAB;
                radObjA_CrossN *= radObjA_CrossN;
                float radObjB_CrossN = radObjB ^ VectorAB;
                radObjB_CrossN *= radObjB_CrossN;
                float invMassSum = objectA.InverseMass + objectB.InverseMass + radObjA_CrossN*objectA.InverseInertia + 
                    radObjB_CrossN * objectB.InverseInertia;

                // Рассчитываем величину импульса силы
                float j = -(1.0f + mixedRestitution) * contactVel;
                j /= invMassSum;
                j /= ContactNumber;

                // Прикладываем импульс к телам
                Vector2D impulse = VectorAB * j;
                objectA.ApplyImpulse(-impulse, radObjA);
                objectB.ApplyImpulse(impulse, radObjB);

                // Импульс силы трения
                relativeVelocity = objectB.Velocity + (objectB.AngularVelocity ^ radObjB) -
                     objectA.Velocity - (objectA.AngularVelocity ^ radObjA);

                Vector2D tangent = relativeVelocity - (VectorAB * (relativeVelocity*VectorAB));
                tangent.Normalize();

                // Рассчитываем касательную величину j
                float jt = -(relativeVelocity * tangent);
                jt /= invMassSum;
                jt /= ContactNumber;

                // Не прикладываем слишком слабые импульсы
                if (Math.Abs(jt) <= TScene.EPS)
                    return;

                // Закон кулона
                Vector2D tangentImpulse;
                if (Math.Abs(jt) < j * mixedStaticFriction)
                    tangentImpulse = tangent * jt;
                else
                    tangentImpulse = tangent * -j * mixedDynamicFriction;

                // Прикладываем импульс трения
                objectA.ApplyImpulse(-tangentImpulse, radObjA);
                objectB.ApplyImpulse(tangentImpulse, radObjB);
            }
        }
        /// <summary>
        /// Функция, обрабатывающая ситуацию, когда оба тела имеют бесконечную массу
        /// </summary>
        private void InfiniteMassCorrection()
        {
            objectA.Velocity.Set(0.0f, 0.0f);
            objectB.Velocity.Set(0.0f, 0.0f);
        }
    }
}
