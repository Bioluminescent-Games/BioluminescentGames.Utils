using UnityEngine;
using UnityEngine.UI;

namespace BackroomsGame.Systems.Settings.UI.Metadata
{
    public class ButtonOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public Button Button { get; private set; }
    }
}
