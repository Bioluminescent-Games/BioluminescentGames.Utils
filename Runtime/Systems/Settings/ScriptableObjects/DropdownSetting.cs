using System.Collections.Generic;
using BioluminescentGames.Utils.Utilities;
using TMPro;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Dropdown Setting", menuName = "Scriptable Objects/Settings/Dropdown Setting")]
    public class DropdownSetting : ValueSetting<uint>
    {
        [SerializeField] private TMP_Dropdown.OptionData[] options;
        public List<TMP_Dropdown.OptionData> Options { get; } = new List<TMP_Dropdown.OptionData>();

        public void AddOptions(params TMP_Dropdown.OptionData[] newOptions) => Options.AddRange(newOptions);

        public override void Initialize()
        {
            AddOptions(options);
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
    }
}
