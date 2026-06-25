using BackroomsGame.UI;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class HorizontalMultiChoiceOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public HorizontalMultiChoice HorizontalMultiChoice { get; private set; }

        private void Awake()
        {
            HorizontalMultiChoice.onValueChanged.AddListener(_ => SetDirty());
        }
    }
}