using UnityEngine;
using UnityEngine.UI;

namespace BackroomsGame.Systems.Settings.UI.Metadata
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