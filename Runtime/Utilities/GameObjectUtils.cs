using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public static class GameObjectUtils
    {
        /// <summary>
        /// Toggles the active state of the GameObject.
        /// </summary>
        /// <param name="obj">The GameObject to toggle activity.</param>
        public static void ToggleActive(this GameObject obj) =>
            obj.SetActive(!obj.activeSelf);

        /// <summary>
        /// Activates the GameObject.
        /// </summary>
        /// <param name="obj">The GameObject to activate.</param>
        public static void Show(this GameObject obj) =>
            obj.SetActive(true);

        /// <summary>
        /// Deactivates the GameObject.
        /// </summary>
        /// <param name="obj">The GameObject to deactivate.</param>
        public static void Hide(this GameObject obj) =>
            obj.SetActive(false);

        /// <summary>
        /// Checks if the GameObject has a component of type T.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if the GameObject has the component, false otherwise.</returns>
        public static bool HasComponent<T>(this Object obj) where T : Component
        {
            return obj.GetComponent<T>();
        }

        /// <summary>
        /// Gets the component of type T if it exists, otherwise adds it to the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="obj">The GameObject to get or add the component to.</param>
        /// <returns>The component of type T.</returns>
        public static T GetOrAddComponent<T>(this Object obj) where T : Component
        {
            if (obj.HasComponent<T>())
                return obj.GetComponent<T>();

            return obj.AddComponent<T>();
        }

        /// <summary>
        /// Adds a component to the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="obj">The GameObject to add the component to.</param>
        /// <returns>The component of type T.</returns>
        public static T AddComponentCustom<T>(this Object obj) where T : Component
        {
            return obj.AddComponent<T>();
        }

        /// <summary>
        /// Returns the object itself if it exists, null otherwise.
        /// </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
        /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
        /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
        /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object being checked.</param>
        /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// Destroys the specified GameObject.
        /// </summary>
        /// <remarks>
        /// This is an extension method wrapper around UnityEngine.Object.Destroy that provides a more convenient syntax.
        /// The GameObject will be destroyed at the end of the current frame.
        /// </remarks>
        /// <param name="obj">The GameObject to destroy.</param>
        public static void Destroy(this GameObject obj)
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
            #else
            Object.Destroy(obj);
            #endif
        }

        /// <summary>
        /// Tries to get a component from the ancestry tree. Also returns the component if it's on the current object.
        /// </summary>
        /// <param name="obj">The base object to get the components from.</param>
        /// <param name="component">The component found</param>
        /// <typeparam name="T">The component type</typeparam>
        /// <returns>True if it found the object</returns>
        public static bool TryGetComponentInAncestry<T>(this GameObject obj, out T component)
        {
            while (true)
            {
                if (obj.TryGetComponent(out component)) return true;
                if (obj.transform.parent != null) continue;

                return false;
            }
        }

        /// <inheritdoc cref="TryGetComponentInAncestry{T}(UnityEngine.GameObject, out T)" />
        public static bool TryGetComponentInAncestry<T>(this Component obj, out T component)
        {
            return obj.gameObject.TryGetComponentInAncestry(out component);
        }

        /// <summary>
        /// Get a component from the ancestry tree. Also returns the component if it's on the current object.
        /// </summary>
        /// <param name="obj">The base object to get the components from.</param>
        /// <typeparam name="T">The component type</typeparam>
        /// <returns>The first component found in the tree.</returns>
        public static T GetComponentInAncestry<T>(this GameObject obj)
        {
            obj.TryGetComponentInAncestry(out T component);
            return component;
        }

        /// <inheritdoc cref="GetComponentInAncestry{T}(UnityEngine.GameObject)" />
        public static T GetComponentInAncestry<T>(this Component obj)
        {
            return obj.gameObject.GetComponentInAncestry<T>();
        }

        /// <summary>
        /// Tries to get a component from any of its children (and itself).
        /// </summary>
        /// <param name="obj">The base object to get the component from</param>
        /// <param name="component">The component if found</param>
        /// <param name="includeInactive">Whether to include inactive objects in the search</param>
        /// <typeparam name="T">The component type</typeparam>
        /// <returns>True if it found the component.</returns>
        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component, bool includeInactive = false)
        {
            component = obj.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        /// <inheritdoc cref="TryGetComponentInChildren{T}(UnityEngine.GameObject, out T, bool)" />
        public static bool TryGetComponentInChildren<T>(this Component obj, out T component, bool includeInactive = false)
        {
            return obj.gameObject.TryGetComponentInChildren(out component, includeInactive);
        }
    }
}
