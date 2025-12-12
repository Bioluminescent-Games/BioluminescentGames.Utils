using ZLinq;
using UnityEngine;

namespace BioluminescentGames.Utils
{
    public static class VectorUtils
    {
        public static Vector3 FindCentre(this Vector3[] points)
        {
            float x = points.AsValueEnumerable().Select(p => p.x).Sum() / points.Length;
            float y = points.AsValueEnumerable().Select(p => p.y).Sum() / points.Length;
            float z = points.AsValueEnumerable().Select(p => p.z).Sum() / points.Length;
            return new Vector3(x, y, z);
        }
        public static Vector2 XY(this Vector3 vector) => new Vector2(vector.x, vector.y);

        public static Vector3 XY0(this Vector2 vector) => new Vector3(vector.x, vector.y, 0.0f);

        public static Vector3 X0Y(this Vector2 vector) => new Vector3(vector.x, 0.0f, vector.y);

        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);

        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null) => new Vector2(x ?? vector.x, y ?? vector.y);

        public static bool IsNear(Vector3 a, Vector3 b, float threshold)
        {
            return IsNear((a - b).sqrMagnitude, threshold);
        }

        public static bool IsNear(float sqrDst, float threshold)
        {
            return sqrDst < threshold * threshold;
        }
    }
}
