using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class KeybindOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public TMP_Text ButtonText { get; private set; }
    }
}
