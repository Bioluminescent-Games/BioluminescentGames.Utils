using UnityEngine;
using UnityEngine.UI;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class ButtonOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public Button Button { get; private set; }
    }
}
