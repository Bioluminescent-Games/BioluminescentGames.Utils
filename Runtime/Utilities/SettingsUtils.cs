using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;

namespace BioluminescentGames.Utils.Runtime
{
    public static class SettingsUtils
    {
        public static void RegisterHandler<T>(this ValueSetting<T> setting, System.Action<T> onValueChanged)
        {
            setting.OnChanged += onValueChanged;
            onValueChanged?.Invoke(setting.Value);
        }
        
        public static void DeregisterHandler<T>(this ValueSetting<T> setting, System.Action<T> onValueChanged)
        {
            setting.OnChanged -= onValueChanged;
        }
    }
}
