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

        protected override void LoadFromPlayerPrefs()
        {
            AddOptions(options);
            InternalValue = (uint)EnhancedPlayerPrefs.GetInt(ID, (int)DefaultValue);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetInt(ID, (int)InternalValue);
        }
    }
}
