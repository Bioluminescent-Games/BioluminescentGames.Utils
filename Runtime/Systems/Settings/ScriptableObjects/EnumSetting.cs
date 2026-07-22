using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.StaticUtilities;
using BioluminescentGames.Utils.Utilities;
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
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
        
        [SerializeField, Tooltip("Will you use scripts to populate the options?")] private bool populateOptionsAtRuntime;
#if EDITOR_ATTRIBUTES
        [HideField(nameof(populateOptionsAtRuntime))]
#endif
        [SerializeField] private EnumSettingOption[] options;
        [field: SerializeField] public DisplayStyle Style { get; private set; }
        
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public bool LocalizedOptions { get; private set; }
#endif

        internal List<EnumSettingOption> Options { get; } = new();
        
        public int Index => IndexOf(Value);
        public int InternalIndex => IndexOf(InternalValue);

        public override void Initialize()
        {
            Options.Clear();
            if (!populateOptionsAtRuntime)
                Options.AddRange(options);
            
            base.Initialize();
        }

        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = EnhancedPlayerPrefs.GetString(IDForSaving, DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetString(IDForSaving, InternalValue);
        }

        public int IndexOf(string id)
        {
            return Options.FindIndex(option => option.id == id);
        }

        public void SetValueByIndex(int index)
        {
            Value = Options[index].id;
        }

        public void AddOptions(params EnumSettingOption[] option)
        {
            Log.Assert(populateOptionsAtRuntime, $"You cannot modify options when {nameof(populateOptionsAtRuntime)} isn't enabled.", this);
            Options.AddRange(option);
            Reload();
        }

        public void AddOption(EnumSettingOption option)
        {
            Log.Assert(populateOptionsAtRuntime, $"You cannot modify options when {nameof(populateOptionsAtRuntime)} isn't enabled.", this);
            Options.Add(option);
            Reload();
        }

        public void ClearOptions()
        {
            Log.Assert(populateOptionsAtRuntime, $"You cannot modify options when {nameof(populateOptionsAtRuntime)} isn't enabled.", this);
            Options.Clear();
            Reload();
        }

        [Serializable]
        public struct EnumSettingOption
        {
#if BG_ENABLE_LOCALIZATION
            public LocalizedString localizedDisplayName;
#endif
            public string displayNameString;
            public string id;
        }
    }
}
