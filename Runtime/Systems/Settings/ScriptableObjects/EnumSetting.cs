using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.Utilities;
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using ZLinq;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Multiple-Choice Setting", menuName = "Scriptable Objects/Settings/Multiple-Choice Setting")]
    public class EnumSetting : ValueSetting<uint>
    {
        public enum DisplayStyle { Dropdown, Horizontal }
        
        [FormerlySerializedAs("options")] 
        [SerializeField, HideInInspector] private TMP_Dropdown.OptionData[] legacyOptions;
        
        #if EDITOR_ATTRIBUTES
        [Rename("Options")]
        #endif
        [SerializeField] private string[] stringOptions;
        [field: SerializeField] public DisplayStyle Style { get; private set; }
        public List<string> Options { get; } = new();

        public void AddOptions(params string[] newOptions) => Options.AddRange(newOptions);

        public override void Initialize()
        {
            AddOptions(stringOptions);
            base.Initialize();
        }

        protected override void LoadFromPlayerPrefs()
        {
            InternalValue = (uint)EnhancedPlayerPrefs.GetInt(IDForSaving, (int)DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetInt(IDForSaving, (int)InternalValue);
        }

        public void AddOptions(Type enumType)
        {
            string[] optionsFromEnum = Enum.GetNames(enumType);
            Options.AddRange(optionsFromEnum);
        }

        private void OnValidate()
        {
            if (legacyOptions == null)
                return;

            if (legacyOptions.Length <= 0)
                return;
            
            stringOptions = legacyOptions.AsValueEnumerable().Select(o => o.text).ToArray();
            legacyOptions = null;
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}
