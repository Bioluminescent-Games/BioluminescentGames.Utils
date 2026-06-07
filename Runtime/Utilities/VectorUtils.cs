#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public static class VectorUtils
    {
        public static Vector3 FindCentre(this Vector3[] points)
        {
            float x = points
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(p => p.x).Average();
            float y = points
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(p => p.y).Average();
            float z = points
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(p => p.z).Average();
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

        /// <summary>
        /// Calculates the squared distance between two vectors, avoiding the overhead of a square root calculation.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        /// <summary>
        /// This is slower than <see cref="SqrDistance"/>.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The distance between the two vectors</returns>
        public static float Distance(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude;
        }

        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        public static bool ApproximatelyZero(Vector3 vector)
        {
            return Approximately(vector, Vector3.zero);
        }

        public static bool ApproximatelyZero(Vector2 vector)
        {
            return Approximately(vector, Vector2.zero);
        }
    }
}
