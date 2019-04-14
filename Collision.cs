using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public static class Collision
    {
        /// <summary>
        /// Делегат задающий процедуру, разрешающую коллизию двух объектов
        /// </summary>
        /// <typeparam name="TManifold"></typeparam>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="curManifold"></param>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        public delegate void CollisionSolver<TManifold, TBody>(TManifold curManifold, TBody objectA, TBody objectB);
        /// <summary>
        /// Диспетчер, определяющий какую процедуру необходимо вызвать
        /// </summary>
        private static CollisionSolver<TManifold, TBody>[,] dispatcher =
        {
            { CircleToCircle, CircleToPolygon },
            { PolygonToCircle, PolygonToPolygon }
        };
        public static CollisionSolver<TManifold, TBody>[,] Dispatcher => dispatcher;
        private static float FindAxisLeastPenetration(out int faceIndex, TPolygon polyA, TPolygon polyB)
        {
            float bestDistance = float.MinValue;
            int bestIndex = 0;

            for (int i = 0; i < polyA.VerticesCount; ++i)
            {
                // Получаем нормаль полигона A
                Vector2D curNormal = polyA.Normals[i];
                Vector2D normalOriented = polyA.MatrOrient * curNormal;

                // Переводим нормаль в систему отсчёта формы B
                TMat22 buT = polyB.MatrOrient.Transpose();
                curNormal = buT * normalOriented;

                // Получаем опорную точку от B вдоль направления -curNormal
                Vector2D curSupport = polyB.GetSupport(-curNormal);

                // Получаем вершину с формы А и переводим её в систему отсчёта формы B
                Vector2D curVertexA = polyA.Vertices[i];
                curVertexA = (polyA.MatrOrient * curVertexA) + polyA.Body.Position;
                curVertexA -= polyB.Body.Position;
                curVertexA = buT * curVertexA;

                // Рассчитаем величину проникновения
                float distance = curNormal * (curSupport - curVertexA);

                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }
            faceIndex = bestIndex;
            return bestDistance;
        }
        private static void FindIncidentFace(Vector2D[] vertices, TPolygon refPoly, TPolygon incPoly, int referenceIndex)
        {
            Vector2D referenceNormal = refPoly.Normals[referenceIndex];

            // Рассчитаем нормаль в системе отсчета полигона "инцидента"
            referenceNormal = refPoly.MatrOrient * referenceNormal; // По отношению к системе отсчёта сцены
            referenceNormal = incPoly.MatrOrient.Transpose() * referenceNormal; // по отношению к системе отсчёта полигона "инцидента"

            // Ищем наиболее "ненормальный" вектор на полигоне "инциденте"
            int incidentFace = 0;
            float minDot = float.MaxValue;
            for (int i = 0; i < incPoly.VerticesCount; ++i)
            {
                float dot = referenceNormal * incPoly.Normals[i];
                if (dot < minDot)
                {
                    minDot = dot;
                    incidentFace = i;
                }
            }

            // Назначаем ближайшие вершины к полигоне "инциденте"
            vertices[0] = incPoly.MatrOrient * incPoly.Vertices[incidentFace] + incPoly.Body.Position;
            incidentFace = incidentFace + 1;
            if (incidentFace == incPoly.VerticesCount)
                incidentFace = 0;
            vertices[1] = incPoly.MatrOrient * incPoly.Vertices[incidentFace] + incPoly.Body.Position;
        }
        private static int Clip(Vector2D normal, float c, Vector2D[] face)
        {
            int sp = 0;
            Vector2D[] outFace = { face[0], face[1] };

            // Получаем расстояния от каждой конечно точки до прямой
            // d = ax + by - c
            float distance1 = (normal * face[0]) - c;
            float distance2 = (normal * face[1]) - c;

            // Если соединение отрицательное (за сценой)
            if (distance1 < 0.0f || Math.Abs(distance1) <= TScene.EPS)
                outFace[sp++] = face[0];
            if (distance2 < 0.0f || Math.Abs(distance1) <= TScene.EPS)
                outFace[sp++] = face[1];

            // Если точки на разных сторонах полигона
            if (distance1 * distance2 < 0.0f) 
            {
                // Добавляем точку пересечения
                float alpha = distance1 / (distance1 - distance2);
                outFace[sp] = face[0] + alpha * (face[1] - face[0]);
                ++sp;
            }

            // Записываем наши новые значения
            face[0] = outFace[0];
            face[1] = outFace[1];

            if (sp == 3)
                throw new Exception("Произошла ошибка в расчётах");

            return sp;
        }
        private static bool BiasGreaterThan(float a, float b)
        {
            const float BIAS_RELATIVE = 0.95f;
            const float BIAS_ABSOLUTE = 0.01f;
            return a > b * BIAS_RELATIVE + a * BIAS_ABSOLUTE || Math.Abs(a - b * BIAS_RELATIVE- a * BIAS_ABSOLUTE) <= TScene.EPS;
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="TBody"/> с формой <see cref="TCircle"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void CircleToCircle(TManifold curManifold, TBody objectA, TBody objectB)
        {
            // Получаем форму тел
            TCircle circleA = objectA.GetShape as TCircle;
            TCircle circleB = objectB.GetShape as TCircle;
            // Вектор из A в B
            Vector2D vectorAB = objectB.Position - objectA.Position;

            float distanceSqr = vectorAB.LengthSquared;
            float sumRadius = circleA.Radius + circleB.Radius;
            // Нет контакта между окружностями
            if (distanceSqr > sumRadius * sumRadius || Math.Abs(distanceSqr - sumRadius * sumRadius) <= TScene.EPS)
            {
                curManifold.ContactNumber = 0;
                return;
            }
            float distance = vectorAB.Length;
            curManifold.ContactNumber = 1;
            // Если окружности имеют одинаковую позицию
            if (distance <= TScene.EPS)
            {
                curManifold.Penetration = circleA.Radius;
                curManifold.VectorAB = new Vector2D(1, 0);
                curManifold.Contacts[0] = objectA.Position;                
            }            
            else
            {
                curManifold.Penetration = circleA.Radius + circleB.Radius - distance;
                curManifold.VectorAB = vectorAB / distance;
                curManifold.Contacts[0] = curManifold.VectorAB * circleA.Radius + objectA.Position;
            }
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="TBody"/> с формами <see cref="TCircle"/> и <see cref="TPolygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void CircleToPolygon(TManifold curManifold, TBody objectA, TBody objectB)
        {
            TCircle circleA = objectA.GetShape as TCircle;
            TPolygon polyB = objectB.GetShape as TPolygon;

            // Перевод центра круга в систему отсчёта полигона
            Vector2D center = objectA.Position;
            center = polyB.MatrOrient.Transpose() * (center - objectB.Position);

            // Найдём ребро с минимальным проникновением в окружность
            float separation = float.MinValue;
            int faceNormal = 0;
            for (int i = 0; i < polyB.VerticesCount; ++i)
            {
                float curSeparation = polyB.Normals[i] * (center - polyB.Vertices[i]);

                if (curSeparation > circleA.Radius)
                    return;

                if (curSeparation > separation)
                {
                    separation = curSeparation;
                    faceNormal = i;
                }
            }

            // Получаем вершины ребра, которое мы выбрали
            Vector2D v1 = polyB.Vertices[faceNormal];
            int sndVertex = faceNormal + 1;
            if (sndVertex >= polyB.VerticesCount)
                sndVertex = 0;
            Vector2D v2 = polyB.Vertices[sndVertex];

            // Если центр внутри полигона
            if (Math.Abs(separation) <= TScene.EPS)
            {
                curManifold.ContactNumber = 1;
                curManifold.VectorAB = -(polyB.MatrOrient * polyB.Normals[faceNormal]);
                curManifold.Contacts[0] = curManifold.VectorAB * circleA.Radius + objectA.Position;
                curManifold.Penetration = circleA.Radius;
                return;
            }

            // Определить в какой области Вороного лежит центр окружности            
            float dot1 = (center - v1) * (v2 - v1);
            float dot2 = (center - v2) * (v1 - v2);
            curManifold.Penetration = circleA.Radius - separation;

            // Ближе к точке v1
            if (dot1 < 0.0f || Math.Abs(dot1) <= TScene.EPS)
            {
                if ((center - v1).LengthSquared > circleA.Radius * circleA.Radius)
                    return;
                curManifold.ContactNumber = 1;
                Vector2D vAB = v1 - center;
                vAB = polyB.MatrOrient * vAB;
                vAB.Normalize();
                curManifold.VectorAB = vAB;
                v1 = polyB.MatrOrient * v1 + objectB.Position;
                curManifold.Contacts[0] = v1;
            }

            // Ближе к v2
            else if (dot2 < 0.0f || Math.Abs(dot2) <= TScene.EPS)
            {
                if ((center - v2).LengthSquared > circleA.Radius * circleA.Radius)
                    return;
                curManifold.ContactNumber = 1;
                Vector2D vAB = v2 - center;
                v2 = polyB.MatrOrient * v2 + objectB.Position;
                curManifold.Contacts[0] = v2;
                vAB = polyB.MatrOrient * vAB;
                vAB.Normalize();
                curManifold.VectorAB = vAB;                
            }

            // Ближе к центральной области
            else
            {
                Vector2D vAB = polyB.Normals[faceNormal];
                if ((center - v1) * vAB > circleA.Radius)
                    return;

                vAB = polyB.MatrOrient * vAB;
                curManifold.VectorAB = -vAB;
                curManifold.Contacts[0] = curManifold.VectorAB * circleA.Radius + objectA.Position;
                curManifold.ContactNumber = 1;
            }
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="TBody"/> с формами <see cref="TCircle"/> и <see cref="TPolygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void PolygonToCircle(TManifold curManifold, TBody objectA, TBody objectB)
        {
            CircleToPolygon(curManifold, objectB, objectA);
            curManifold.VectorAB = -curManifold.VectorAB;
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="TBody"/> с формой <see cref="TPolygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void PolygonToPolygon(TManifold curManifold, TBody objectA, TBody objectB)
        {
            TPolygon polyA = objectA.GetShape as TPolygon;
            TPolygon polyB = objectB.GetShape as TPolygon;
            curManifold.ContactNumber = 0;

            // Ищем разделяющую ось среди с гранями полигона А
            int faceA;
            float penetrationA = FindAxisLeastPenetration(out faceA, polyA, polyB);
            if (penetrationA > 0.0f || Math.Abs(penetrationA) <= TScene.EPS)
                return;

            // Ищем разделяющую ось среди с гранями полигона В
            int faceB;
            float penetrationB = FindAxisLeastPenetration(out faceB, polyB, polyA);
            if (penetrationB > 0.0f || Math.Abs(penetrationA) <= TScene.EPS)
                return;

            int referenceIndex;
            bool flip; // Всегда указывает из объекта А в объект В

            TPolygon RefPoly; 
            TPolygon IncPoly; // Инцидентный полигон

            // Определим какая фигура содержит торец полигона Ref 
            if (BiasGreaterThan(penetrationA, penetrationB))
            {
                RefPoly = polyA;
                IncPoly = polyB;
                referenceIndex = faceA;
                flip = false;
            }

            else
            {
                RefPoly = polyB;
                IncPoly = polyA;
                referenceIndex = faceB;
                flip = true;
            }

            // Определяем торец инцидентного полигона
            Vector2D[] incidentFace = new Vector2D[2];
            FindIncidentFace(incidentFace, RefPoly, IncPoly, referenceIndex);

            // Устанавливаем вершины торца полигона Ref
            Vector2D v1 = RefPoly.Vertices[referenceIndex];
            referenceIndex = referenceIndex + 1;
            if (referenceIndex == RefPoly.VerticesCount)
                referenceIndex = 0;
            Vector2D v2 = RefPoly.Vertices[referenceIndex];

            // Переводим вершины в пространство сцены
            v1 = (RefPoly.MatrOrient * v1) + RefPoly.Body.Position;
            v2 = (RefPoly.MatrOrient * v2) + RefPoly.Body.Position;

            // Рассчитаем нормаль к лицевой грани ref полигона
            Vector2D sidePlaneNormal = v2 - v1;
            sidePlaneNormal.Normalize();

            Vector2D refFaceNormal = new Vector2D(sidePlaneNormal.Y, -sidePlaneNormal.X);

            // ax + by = c
            // c расстояние от тела
            float refC = refFaceNormal* v1;
            float negSide = -sidePlaneNormal * v1;
            float posSide = sidePlaneNormal* v2;

            // Присоединим переднюю грань инцидентного полинома к боковым граням ref полинома
            if (Clip(-sidePlaneNormal, negSide, incidentFace) < 2)
                return; 

            if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
                return;

            // Переворачиваем нормаль
            if (flip)
                curManifold.VectorAB = -refFaceNormal;
            else
                curManifold.VectorAB = refFaceNormal;

            int clippedPoints = 0;
            float separation = (refFaceNormal*incidentFace[0]) - refC;
            if (separation < 0.0f || Math.Abs(separation) <= TScene.EPS)
            {
                curManifold.Contacts[clippedPoints] = incidentFace[0];
                curManifold.Penetration = -separation;
                ++clippedPoints;
            }
            else
                curManifold.Penetration = 0;

            separation = (refFaceNormal* incidentFace[1]) - refC;
            if (separation < 0.0f || Math.Abs(separation) <= TScene.EPS)
            {
                curManifold.Contacts[clippedPoints] = incidentFace[1];

                curManifold.Penetration -= separation;
                ++clippedPoints;
                curManifold.Penetration /= clippedPoints;
            }

            curManifold.ContactNumber = clippedPoints;
        }
    }
}

