using Unity.VisualScripting;
using UnityEngine;

namespace BioluminescentGames.Utils
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
            return obj.GetComponent<T>() ?? obj.AddComponent<T>();
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
        public static void Destroy(this GameObject obj) => Object.Destroy(obj);
    }
}