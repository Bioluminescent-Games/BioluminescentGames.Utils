using BioluminescentGames.Utils.Runtime;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif
#if BG_ENABLE_LOCALIZATION
using Unity.Localization;
using Unity.Localization.Components;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public abstract class EnumOptionUIMetadata : OptionUIMetadata
    {
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public LocalizeStringListEvent DropdownItems { get; private set; }

        public void SetItems(LocalizedString[] items)
        {
            var group = new LocalizedStringGroup();
            
            items
#if ZLINQ
                .AsValueEnumerable()
#endif
                .CopyTo(group.Strings);

            DropdownItems.ListReference = group;
            
            DropdownItems.RefreshList();
        }
#endif

        public abstract void SetItems(string[] items);
    }
}
