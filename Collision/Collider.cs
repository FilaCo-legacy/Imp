using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysEngine
{
    public static class Collider
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
        private static CollisionSolver<Manifold, Body>[,] dispatcher =
        {
            { CircleToCircle, CircleToPolygon },
            { PolygonToCircle, PolygonToPolygon }
        };
        public static CollisionSolver<Manifold, Body>[,] Dispatcher => dispatcher;
        private static float FindAxisLeastPenetration(out int faceIndex, Polygon polyA, Polygon polyB)
        {
            float bestDistance = float.MinValue;
            int bestIndex = 0;

            for (int i = 0; i < polyA.VerticesCount; ++i)
            {
                // Получаем нормаль полигона A
                Vector curNormal = polyA.Normals[i];
                Vector normalOriented = polyA.MatrixOrient * curNormal;

                // Переводим нормаль в систему отсчёта формы B
                TMat22 buT = polyB.MatrixOrient.Transpose();
                curNormal = buT * normalOriented;

                // Получаем опорную точку от B вдоль направления -curNormal
                Vector curSupport = polyB.GetSupport(-curNormal);

                // Получаем вершину с формы А и переводим её в систему отсчёта формы B
                Vector curVertexA = polyA.Vertices[i];
                curVertexA = (polyA.MatrixOrient * curVertexA) + polyA.Body.Position;
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
        private static void FindIncidentFace(Vector[] vertices, Polygon refPoly, Polygon incPoly, int referenceIndex)
        {
            Vector referenceNormal = refPoly.Normals[referenceIndex];

            // Рассчитаем нормаль в системе отсчета полигона "инцидента"
            referenceNormal = refPoly.MatrixOrient * referenceNormal; // По отношению к системе отсчёта сцены
            referenceNormal = incPoly.MatrixOrient.Transpose() * referenceNormal; // по отношению к системе отсчёта полигона "инцидента"

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
            vertices[0] = incPoly.MatrixOrient * incPoly.Vertices[incidentFace] + incPoly.Body.Position;
            incidentFace = incidentFace + 1;
            if (incidentFace == incPoly.VerticesCount)
                incidentFace = 0;
            vertices[1] = incPoly.MatrixOrient * incPoly.Vertices[incidentFace] + incPoly.Body.Position;
        }
        private static int Clip(Vector normal, float c, Vector[] face)
        {
            int sp = 0;
            Vector[] outFace = { face[0], face[1] };

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
        /// Разрешение коллизии для двух физических тел <see cref="Body"/> с формой <see cref="Circle"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void CircleToCircle(Manifold curManifold, Body objectA, Body objectB)
        {
            // Получаем форму тел
            Circle circleA = objectA.GetShape as Circle;
            Circle circleB = objectB.GetShape as Circle;
            // Вектор из A в B
            Vector vectorAB = objectB.Position - objectA.Position;

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
                curManifold.VectorAB = new Vector(1, 0);
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
        /// Разрешение коллизии для двух физических тел <see cref="Body"/> с формами <see cref="Circle"/> и <see cref="Polygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void CircleToPolygon(Manifold curManifold, Body objectA, Body objectB)
        {
            Circle circleA = objectA.GetShape as Circle;
            Polygon polyB = objectB.GetShape as Polygon;

            // Перевод центра круга в систему отсчёта полигона
            Vector center = objectA.Position;
            center = polyB.MatrixOrient.Transpose() * (center - objectB.Position);

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
            Vector v1 = polyB.Vertices[faceNormal];
            int sndVertex = faceNormal + 1;
            if (sndVertex >= polyB.VerticesCount)
                sndVertex = 0;
            Vector v2 = polyB.Vertices[sndVertex];

            // Если центр внутри полигона
            if (Math.Abs(separation) <= TScene.EPS)
            {
                curManifold.ContactNumber = 1;
                curManifold.VectorAB = -(polyB.MatrixOrient * polyB.Normals[faceNormal]);
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
                Vector vAB = v1 - center;
                vAB = polyB.MatrixOrient * vAB;
                vAB.Normalize();
                curManifold.VectorAB = vAB;
                v1 = polyB.MatrixOrient * v1 + objectB.Position;
                curManifold.Contacts[0] = v1;
            }

            // Ближе к v2
            else if (dot2 < 0.0f || Math.Abs(dot2) <= TScene.EPS)
            {
                if ((center - v2).LengthSquared > circleA.Radius * circleA.Radius)
                    return;
                curManifold.ContactNumber = 1;
                Vector vAB = v2 - center;
                v2 = polyB.MatrixOrient * v2 + objectB.Position;
                curManifold.Contacts[0] = v2;
                vAB = polyB.MatrixOrient * vAB;
                vAB.Normalize();
                curManifold.VectorAB = vAB;                
            }

            // Ближе к центральной области
            else
            {
                Vector vAB = polyB.Normals[faceNormal];
                if ((center - v1) * vAB > circleA.Radius)
                    return;

                vAB = polyB.MatrixOrient * vAB;
                curManifold.VectorAB = -vAB;
                curManifold.Contacts[0] = curManifold.VectorAB * circleA.Radius + objectA.Position;
                curManifold.ContactNumber = 1;
            }
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="Body"/> с формами <see cref="Circle"/> и <see cref="Polygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void PolygonToCircle(Manifold curManifold, Body objectA, Body objectB)
        {
            CircleToPolygon(curManifold, objectB, objectA);
            curManifold.VectorAB = -curManifold.VectorAB;
        }
        /// <summary>
        /// Разрешение коллизии для двух физических тел <see cref="Body"/> с формой <see cref="Polygon"/>
        /// </summary>
        /// <param name="curManifold">Многообразие двух объектов</param>
        /// <param name="objectA">Первый объект, входящий в многообразие</param>
        /// <param name="objectB">Второй объект, входящий в многообразие</param>
        public static void PolygonToPolygon(Manifold curManifold, Body objectA, Body objectB)
        {
            Polygon polyA = objectA.GetShape as Polygon;
            Polygon polyB = objectB.GetShape as Polygon;
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

            Polygon RefPoly; 
            Polygon IncPoly; // Инцидентный полигон

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
            Vector[] incidentFace = new Vector[2];
            FindIncidentFace(incidentFace, RefPoly, IncPoly, referenceIndex);

            // Устанавливаем вершины торца полигона Ref
            Vector v1 = RefPoly.Vertices[referenceIndex];
            referenceIndex = referenceIndex + 1;
            if (referenceIndex == RefPoly.VerticesCount)
                referenceIndex = 0;
            Vector v2 = RefPoly.Vertices[referenceIndex];

            // Переводим вершины в пространство сцены
            v1 = (RefPoly.MatrixOrient * v1) + RefPoly.Body.Position;
            v2 = (RefPoly.MatrixOrient * v2) + RefPoly.Body.Position;

            // Рассчитаем нормаль к лицевой грани ref полигона
            Vector sidePlaneNormal = v2 - v1;
            sidePlaneNormal.Normalize();

            Vector refFaceNormal = new Vector(sidePlaneNormal.Y, -sidePlaneNormal.X);

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

