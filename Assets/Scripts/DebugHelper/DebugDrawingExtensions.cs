using System;
using UnityEngine;

namespace Assets.Scripts.DebugHelper
{
    internal static class DebugDrawingExtensions
    {
        public static void DrawArrowHead(Vector2 basePos, Vector2 dir, float size)
        {
            dir.Normalize();
            Vector2 normal = new Vector2(-dir.y, dir.x) * size;
            Gizmos.DrawLine(basePos - normal, basePos + normal);
            Gizmos.DrawLine(basePos - normal, basePos + dir * size);
            Gizmos.DrawLine(basePos + normal, basePos + dir * size);
        }

        public static void DrawArrowHeadFromSpike(Vector2 pointyPos, Vector2 dir, float size)
        {
            dir.Normalize();
            pointyPos -= dir * size;
            Vector2 normal = new Vector2(-dir.y, dir.x) * size;
            Gizmos.DrawLine(pointyPos - normal, pointyPos + normal);
            Gizmos.DrawLine(pointyPos - normal, pointyPos + dir * size);
            Gizmos.DrawLine(pointyPos + normal, pointyPos + dir * size);
        }

        public static void DrawArrow(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            float length = dir.magnitude;
            dir /= length;
            Vector3 baseA = start + dir * (length - 0.1f);
            Gizmos.DrawLine(start, baseA);

            Vector3 normal = new Vector3(-dir.y, dir.x) * 0.1f;
            Gizmos.DrawLine(baseA - normal, baseA + normal);
            Gizmos.DrawLine(baseA - normal, end);
            Gizmos.DrawLine(baseA + normal, end);
        }

        public static void DrawCircle(Vector2 center, float radius = 0.05f)
        {
            int segmentCount = 10;
            Vector2 prevPoint = center + Vector2.up * radius;

            for (float t = 1; t <= segmentCount; t++)
            {
                float x = (t / segmentCount) * Mathf.PI * 2.0f;
                float cx = Mathf.Sin(x);
                float cy = Mathf.Cos(x);

                Vector2 point = center + new Vector2(cx, cy) * radius;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
            Gizmos.DrawLine(prevPoint, center + Vector2.up * radius);
        }

        public static void DrawBezierConnection(Vector2 start, Vector2 end, bool biDirectional)
        {
            Vector2 cp;
            var tangent = (end - start);
            var length = tangent.magnitude;
            var normal = new Vector2(-tangent.y, tangent.x) / length;
            cp = start + tangent * 0.5f + normal * (length / 5f);

            DrawBezierConnection(start, end, cp, biDirectional);
        }

        public static void DrawBezierConnection(Vector2 start, Vector2 end, Vector2 cp, bool biDirectional)
        {
            float arcLength = (Vector2.Distance(end, start) * 2 + Vector2.Distance(end, cp) + Vector2.Distance(start, cp)) / 3f;
            int numberOfSegments = Mathf.CeilToInt(arcLength);
            Vector2 prev = start;
            float t;
            for (t = 1; t <= numberOfSegments; t++)
            {
                Vector2 v = QuadraticBezierCurve(t / numberOfSegments, start, cp, end);
                Gizmos.DrawLine(prev, v);
                prev = v;
            }

            //draw arrows
            t = (numberOfSegments - 1) / (float)numberOfSegments;
            Vector2 dir = end - QuadraticBezierCurve(t, start, cp, end);
            DrawArrowHeadFromSpike(end, dir, 0.3f);
            if (biDirectional)
            {
                dir = start - QuadraticBezierCurve(1f / numberOfSegments, start, cp, end);
                DrawArrowHeadFromSpike(start, dir, 0.3f);
            }
        }

        public static void DrawBezierConnectionWithOffset(Vector2 start, Vector2 end, Vector2 cp, Vector2 offset)
        {
            float arcLength = (Vector2.Distance(end, start) * 2 + Vector2.Distance(end, cp) + Vector2.Distance(start, cp)) / 3f;
            int numberOfSegments = Mathf.CeilToInt(arcLength);
            Vector2 prev = start + offset;
            Gizmos.DrawLine(start, prev);
            float t;
            for (t = 1; t <= numberOfSegments; t++)
            {
                Vector2 v = QuadraticBezierCurve(t / numberOfSegments, start, cp, end) + offset;
                Gizmos.DrawLine(v, v - offset);
                Gizmos.DrawLine(prev, v);
                prev = v;
            }
        }

        public static void DrawProjectileArc(Vector2 start, Vector2 end, float hSpeed, bool isBidiretional)
        {
            float hDelta = end.x - start.x;
            float t = hDelta / hSpeed;
            int numberOfSegments = Mathf.CeilToInt(t) + 4;
            float p0 = start.y - end.y;
            float v0 = 9.81f * t * 0.5f - p0 / t;

            Func<float, float> func = x => 0.5f * -9.81f * x * x + v0 * x + p0;

            Vector2 prev = start;
            float z;
            float timePerSegment = t / numberOfSegments;
            for (z = 1; z <= numberOfSegments; z++)
            {
                Vector2 v = new Vector2(start.x + z * timePerSegment * hSpeed, end.y + func(z * timePerSegment));
                Gizmos.DrawLine(prev, v);
                prev = v;
            }

            Vector2 dir = end - new Vector2(start.x + (numberOfSegments - 1) * timePerSegment * hSpeed, end.y + func((numberOfSegments - 1) * timePerSegment));
            DrawArrowHeadFromSpike(end, dir, 0.3f);
            if (isBidiretional)
            {
                dir = start - new Vector2(start.x + timePerSegment * hSpeed, end.y + func(timePerSegment));
                DrawArrowHeadFromSpike(start, dir, 0.3f);
            }
        }

        public static void DrawProjectileArcWithOffset(Vector2 start, Vector2 end, float hSpeed, Vector2 offset)
        {
            float hDelta = end.x - start.x;
            float t = hDelta / hSpeed;
            int numberOfSegments = Mathf.CeilToInt(t) + 4;
            float p0 = start.y - end.y;
            float v0 = 9.81f * t * 0.5f - p0 / t;

            Func<float, float> func = x => 0.5f * -9.81f * x * x + v0 * x + p0;

            Vector2 prev = start + offset;
            Gizmos.DrawLine(start, prev);
            float z;
            float timePerSegment = t / numberOfSegments;
            for (z = 1; z <= numberOfSegments; z++)
            {
                Vector2 v = new Vector2(start.x + z * timePerSegment * hSpeed, end.y + func(z * timePerSegment)) + offset;
                Gizmos.DrawLine(prev, v);
                Gizmos.DrawLine(v, v - offset);
                prev = v;
            }
        }

        private static Vector2 QuadraticBezierCurve(float t, Vector2 a, Vector2 b, Vector2 c)
        {
            return (1 - t) * (1 - t) * a + 2 * (1 - t) * t * b + t * t * c;
        }

        public static Color LinearBlendBetweenColors(float value, params Color[] colors)
        {
            value = Mathf.Clamp01(value);
            int index = (int)(value * (colors.Length - 1));
            float t = (value * (colors.Length - 1)) - index;
            Color b = index >= colors.Length - 1 ? colors[index] : colors[index + 1];
            return Color.Lerp(colors[index], b, t);
        }

        public static void DrawRect(Vector2 position, Vector2 size)
        {
            Gizmos.DrawLine(position + size, new Vector3(position.x, position.y + size.y));
            Gizmos.DrawLine(position, new Vector3(position.x, position.y + size.y));
            Gizmos.DrawLine(position + size, new Vector3(position.x + size.x, position.y));
            Gizmos.DrawLine(position, new Vector3(position.x + size.x, position.y));
        }

        public static void DrawRect(Rect rect)
        {
            Gizmos.DrawLine(rect.position + rect.size, new Vector3(rect.position.x, rect.position.y + rect.size.y));
            Gizmos.DrawLine(rect.position, new Vector3(rect.position.x, rect.position.y + rect.size.y));
            Gizmos.DrawLine(rect.position + rect.size, new Vector3(rect.position.x + rect.size.x, rect.position.y));
            Gizmos.DrawLine(rect.position, new Vector3(rect.position.x + rect.size.x, rect.position.y));
        }
    }
}
