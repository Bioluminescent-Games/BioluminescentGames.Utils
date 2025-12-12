using System;
using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace BioluminescentGames.Utils
{
    public static class TransformUtils
    {
        public static void SetWorldX(this Transform transform, float x)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.x = x;
            transform.position = oldPosition;
        }
        public static void SetWorldY(this Transform transform, float y)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.y = y;
            transform.position = oldPosition;
        }
        public static void SetWorldZ(this Transform transform, float z)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.z = z;
            transform.position = oldPosition;
        }
        public static void SetLocalX(this Transform transform, float x)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.x = x;
            transform.localPosition = oldPosition;
        }
        public static void SetLocalY(this Transform transform, float y)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.y = y;
            transform.localPosition = oldPosition;
        }
        public static void SetLocalZ(this Transform transform, float z)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.z = z;
            transform.localPosition = oldPosition;
        }

        public static void ChangeWorldX(this Transform transform, float x)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.x += x;
            transform.position = oldPosition;
        }
        public static void ChangeWorldY(this Transform transform, float y)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.y += y;
            transform.position = oldPosition;
        }
        public static void ChangeWorldZ(this Transform transform, float z)
        {
            Vector3 oldPosition = transform.position;
            oldPosition.z += z;
            transform.position = oldPosition;
        }
        public static void ChangeLocalX(this Transform transform, float x)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.x += x;
            transform.localPosition = oldPosition;
        }
        public static void ChangeLocalY(this Transform transform, float y)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.y += y;
            transform.localPosition = oldPosition;
        }
        public static void ChangeLocalZ(this Transform transform, float z)
        {
            Vector3 oldPosition = transform.localPosition;
            oldPosition.z += z;
            transform.localPosition = oldPosition;
        }

        public static Vector3[] TransformAllPoints(this Transform transform, Vector3[] points)
        {
            return points.AsValueEnumerable().Select(transform.TransformPoint).ToArray();
        }

        public static void ClearAllChildren(this Transform transform)
        {
            ClearChildren(transform, (t) => true);
        }
        public static void ClearChildren(this Transform transform, Func<Transform, bool> predicate)
        {
            foreach (Transform child in transform)
                if (predicate(child))
                    UnityEngine.Object.Destroy(child.gameObject);
        }

        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                yield return child;
            }
        }

        public static void SetWorldScale(this Transform transform, Vector3 worldScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(worldScale.x / transform.lossyScale.x, worldScale.y / transform.lossyScale.y, worldScale.z / transform.lossyScale.z);
        }

        public static RectTransform AsRectTransform(this Transform transform) => transform as RectTransform;

        public static void InterpolateLocal(this Transform transform, Vector3? positionTarget = null,
            float positionSpeed = 1.0f, Quaternion? rotationTarget = null, float rotationSpeed = 1.0f,
            bool slerpRotation = true, Vector3? scaleTarget = null, float scaleSpeed = 1.0f)
        {
            if (positionTarget.HasValue)
                transform.localPosition = Vector3.Lerp(transform.localPosition, positionTarget.Value, positionSpeed * Time.deltaTime);

            if (rotationTarget.HasValue)
            {
                transform.localRotation = slerpRotation
                    ? Quaternion.Slerp(transform.localRotation, rotationTarget.Value, rotationSpeed * Time.deltaTime)
                    : Quaternion.Lerp(transform.localRotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
            }

            if (scaleTarget.HasValue)
                transform.localScale = Vector3.Lerp(transform.localScale, scaleTarget.Value, scaleSpeed * Time.deltaTime);
        }

        public static void InterpolateWorld(this Transform transform, Vector3? positionTarget = null,
            float positionSpeed = 1.0f, Quaternion? rotationTarget = null, float rotationSpeed = 1.0f,
            bool slerpRotation = true, Vector3? scaleTarget = null, float scaleSpeed = 1.0f)
        {
            if (positionTarget.HasValue)
                transform.position = Vector3.Lerp(transform.position, positionTarget.Value, positionSpeed * Time.deltaTime);

            if (rotationTarget.HasValue)
            {
                transform.rotation = slerpRotation
                    ? Quaternion.Slerp(transform.rotation, rotationTarget.Value, rotationSpeed * Time.deltaTime)
                    : Quaternion.Lerp(transform.rotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
            }

            if (scaleTarget.HasValue)
                transform.localScale = Vector3.Lerp(transform.localScale, scaleTarget.Value, scaleSpeed * Time.deltaTime);
        }
    }
}
