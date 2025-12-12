using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BioluminescentGames.Utils
{
    public static class BoundsUtils
    {
        public static void SetBounds(this BoxCollider collider, Bounds bounds)
        {
            collider.center = bounds.center;
            collider.size = bounds.extents;
        }

        public static void EncapsulateAll(this ref Bounds bounds, IEnumerable<Bounds> boundsToEncapsulate)
        {
            foreach (Bounds encapsulatingBounds in boundsToEncapsulate)
                bounds.Encapsulate(encapsulatingBounds);
        }

        public static void ConfineVector(this Bounds bounds, ref Vector3 vector, bool confineX = true, bool confineY = true, bool confineZ = true)
        {
            if (confineX)
            {
                if (vector.x > bounds.max.x)
                    vector.x = bounds.max.x;

                if (vector.x < bounds.min.x)
                    vector.x = bounds.min.x;
            }

            if (confineY)
            {
                if (vector.y > bounds.max.y)
                    vector.y = bounds.max.y;
                if (vector.y < bounds.min.y)
                    vector.y = bounds.min.y;
            }

            if (confineZ)
            {
                if (vector.z > bounds.max.z)
                    vector.z = bounds.max.z;

                if (vector.z < bounds.min.z)
                    vector.z = bounds.min.z;
            }
        }

        public static Bounds GetBoundsEncapsulating(IEnumerable<Bounds> boundsToEncapsulate)
        {
            IEnumerable<Bounds> boundsEnumerable = boundsToEncapsulate as Bounds[] ?? boundsToEncapsulate.ToArray();
            Bounds bounds = boundsEnumerable.ToArray()[0];
            bounds.EncapsulateAll(boundsEnumerable);
            return bounds;
        }

        public static void DrawGizmo(this Bounds bounds)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = oldColor;
        }

        public static Bounds Translate(this Bounds bounds, Transform transform)
        {
            bounds.center = transform.TransformPoint(bounds.center);
            return bounds;
        }

        public static Bounds InverseTranslate(this Bounds bounds, Transform transform)
        {
            bounds.center = transform.InverseTransformPoint(bounds.center);
            return bounds;
        }
    }
}