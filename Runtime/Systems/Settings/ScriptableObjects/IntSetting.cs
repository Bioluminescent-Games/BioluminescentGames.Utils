using BioluminescentGames.Utils.Utilities;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Whole Number Setting", menuName = "Scriptable Objects/Settings/Whole Number Setting")]
    public class IntSetting : ValueSetting<int>
    {
        [field: SerializeField] public int MinValue { get; private set; }
        [field: SerializeField] public int MaxValue { get; private set; }

        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = EnhancedPlayerPrefs.GetInt(IDForSaving, DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetInt(IDForSaving, InternalValue);
        }
    }
}
