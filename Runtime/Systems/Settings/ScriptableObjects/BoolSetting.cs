using BioluminescentGames.Utils.Utilities;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Toggle Setting", menuName = "Scriptable Objects/Settings/Toggle Setting")]
    public class BoolSetting : ValueSetting<bool>
    {
        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = EnhancedPlayerPrefs.GetBool(IDForSaving, DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetBool(IDForSaving, InternalValue);
        }
    }
}
