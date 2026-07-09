using System;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Multiple-Choice Setting", menuName = "Scriptable Objects/Settings/Multiple-Choice Setting")]
    public class EnumSetting : ValueSetting<string>
    {
        public enum DisplayStyle { Dropdown, Horizontal }
        
        [field: SerializeField] public EnumSettingOption[] Options { get; private set; }
        [field: SerializeField] public DisplayStyle Style { get; private set; }

        public int Index => IndexOf(Value);
        public int InternalIndex => IndexOf(InternalValue);

        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = EnhancedPlayerPrefs.GetString(IDForSaving, DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetString(IDForSaving, InternalValue);
        }

        [Serializable]
        public struct EnumSettingOption
        {
#if BG_ENABLE_LOCALIZATION
            public LocalizedString displayName;
#else
            public string displayName;
#endif
            public string id;
        }

        public int IndexOf(string id)
        {
            return Array.FindIndex(Options, option => option.id == id);
        }

        public void SetValueByIndex(int index)
        {
            Value = Options[index].id;
        }
    }
}
