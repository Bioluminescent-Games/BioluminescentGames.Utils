using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.Utilities;
#if UNITY_EDITOR
using UnityEditor;
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
        
        [SerializeField] private EnumSettingOption options;
        [field: SerializeField] public DisplayStyle Style { get; private set; }
        public List<EnumSettingOption> Options { get; } = new();

        public int Index => IndexOf(Value);
        public int InternalIndex => IndexOf(InternalValue);

#if UNITY_EDITOR
        private EnumSettingOption[] _optionsBackup;
#endif

        public void AddOptions(params EnumSettingOption[] newOptions) => Options.AddRange(newOptions);

        public override void Initialize()
        {
            AddOptions(options);
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

        protected override void OnEnable()
        {
            base.OnEnable();
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _optionsBackup = Options.ToArray();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Options.Clear();
                    Options.AddRange(_optionsBackup);
                    break;
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
#endif

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
            return Options.FindIndex(option => option.id == id);
        }

        public void SetValueByIndex(int index)
        {
            Value = Options[index].id;
        }
    }
}
