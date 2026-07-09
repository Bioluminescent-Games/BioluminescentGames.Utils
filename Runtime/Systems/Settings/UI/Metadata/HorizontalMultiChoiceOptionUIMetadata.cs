using BackroomsGame.UI;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class HorizontalMultiChoiceOptionUIMetadata : EnumOptionUIMetadata
    {
        [field: SerializeField] public HorizontalMultiChoice HorizontalMultiChoice { get; private set; }

        private void Awake()
        {
            HorizontalMultiChoice.onValueChanged.AddListener(_ => SetDirty());
        }
        
        public override void SetItems(string[] items)
        {
            HorizontalMultiChoice.ClearOptions();
            HorizontalMultiChoice.AddOptions(items
#if ZLINQ
                .AsValueEnumerable()
#endif
                .ToList());
        }
    }
}