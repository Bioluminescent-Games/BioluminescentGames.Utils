using BioluminescentGames.Utils.Runtime;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public abstract class EnumOptionUIMetadata : OptionUIMetadata
    {
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public LocalizeStringListEvent DropdownItems { get; private set; }

        public void SetItems(LocalizedString[] items)
        {
            DropdownItems.ListReference = new LocalizedStringGroup
            {
                Strings = items
#if ZLINQ
                    .AsValueEnumerable()
#endif
                    .ToList(),
            };
            
            DropdownItems.RefreshList();
        }
#endif

        public abstract void SetItems(string[] items);
    }
}
