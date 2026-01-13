using TMPro;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class DropdownOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public TMP_Dropdown Dropdown { get; private set; }

        private void Awake()
        {
            Dropdown.onValueChanged.AddListener(_ => SetDirty());
        }
    }
}