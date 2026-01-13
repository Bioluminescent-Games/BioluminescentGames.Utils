using UnityEngine;
using UnityEngine.UI;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class CheckboxOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public Toggle Checkbox { get; private set; }

        private void Awake()
        {
            Checkbox.onValueChanged.AddListener(_ => SetDirty());
        }
    }
}