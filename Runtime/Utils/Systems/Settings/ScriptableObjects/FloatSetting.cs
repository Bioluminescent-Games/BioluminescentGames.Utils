using BioluminescentGames.Utils.Utilities;
using UnityEngine;

namespace BackroomsGame.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Decimal Number Setting",
        menuName = "Scriptable Objects/Settings/Decimal Number Setting")]
    public class FloatSetting : ValueSetting<float>
    {
        [field: SerializeField] public float MinValue { get; private set; }
        [field: SerializeField] public float MaxValue { get; private set; }

        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = EnhancedPlayerPrefs.GetFloat(ID, DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetFloat(ID, InternalValue);
        }
    }
}
