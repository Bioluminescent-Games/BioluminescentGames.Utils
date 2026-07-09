using TMPro;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class DropdownOptionUIMetadata : EnumOptionUIMetadata
    {
        [field: SerializeField] public TMP_Dropdown Dropdown { get; private set; }
        
        private void Awake()
        {
            Dropdown.onValueChanged.AddListener(_ => SetDirty());
        }

        public void SetItems(string[] items)
        {
            Dropdown.ClearOptions();
            Dropdown.AddOptions(items
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(s => new TMP_Dropdown.OptionData(s))
                .ToList());
        }
    }
}