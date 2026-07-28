using System;
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
            _allSettings = Resources.LoadAll<Setting>("Settings");

            foreach (ISetting setting in _allSettings)
            {
                Log.Trace($"Load setting {setting.ID}");
                setting.Initialize();
            }

            Log.Verbose("Settings > Loaded Settings!");
        }

        private static Setting[] _allSettings;
        
        /// <summary>
        /// Get a setting of type
        /// </summary>
        /// <param name="id">The ID of the setting</param>
        /// <typeparam name="T">The type of setting e.g. <see cref="ButtonSetting"/>, <see cref="Setting"/>, <see cref="ValueSetting{T}"/>, <see cref="FloatSetting"/> etc.</typeparam>
        /// <returns>The setting</returns>
        public static T Get<T>(string id) where T : class, ISetting => Get(id) as T;
        
        /// <summary>
        /// Gets a setting by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the setting.</param>
        /// <returns>The setting with the specified identifier, or null if not found.</returns>
        public static ISetting Get(string id) => Array.Find(_allSettings, setting => setting.ID == id);
        
        /// <summary>
        /// Get all settings
        /// </summary>
        /// <returns>An array of all settings</returns>
        public static Setting[] GetAll() => _allSettings;
    }
}
