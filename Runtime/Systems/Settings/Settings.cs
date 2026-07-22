using BioluminescentGames.Utils.StaticUtilities;
using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings
{
    public static class Settings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Awake()
        {
            Resources.LoadAll<Setting>("Settings");

            foreach (ISetting setting in GetAll())
            {
                Log.Trace($"Load setting {setting.ID}");
                setting.Initialize();
            }

            Log.Verbose("Settings > Loaded Settings!");
        }

        /// <summary>
        /// Get a setting of type
        /// </summary>
        /// <param name="id">The ID of the setting</param>
        /// <typeparam name="T">The type of setting e.g. <see cref="ButtonSetting"/>, <see cref="Setting"/>, <see cref="ValueSetting{T}"/>, <see cref="FloatSetting"/> etc.</typeparam>
        /// <returns>The setting</returns>
        public static T Get<T>(string id) where T : class, ISetting => Setting.Get(id) as T;
        
        /// <summary>
        /// Get a setting as ISetting
        /// </summary>
        /// <param name="id">The ID of the setting</param>
        /// <returns>The Setting</returns>
        public static ISetting Get(string id) => Setting.Get(id);
        
        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>An array of all settings</returns>
        public static ISetting[] GetAll() => Setting.GetAll();
    }
}
