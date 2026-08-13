#if BG_ENABLE_LOCALIZATION
using BioluminescentGames.Utils.StaticUtilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace BioluminescentGames.Utils.Runtime
{
    public static class LocalizationUtils
    {
        public static void RefreshList(this LocalizeStringListEvent localizeStringListEvent)
        {
            switch (localizeStringListEvent.ListReference)
            {
                case LocalizedStringGroup group:
                    group.RefreshList();
                    break;
                case LocalizedStringList list:
                    list.RefreshString();
                    break;
            }
        }
        
        public static void RefreshList(this LocalizedStringGroup localizedStringGroup)
        {
            if (localizedStringGroup == null)
                throw new System.ArgumentNullException(nameof(localizedStringGroup));

            if (localizedStringGroup.Strings.Count <= 0)
            {
                Log.Warning($"Cannot refresh {nameof(LocalizedStringGroup)} when {nameof(localizedStringGroup.Strings)} is empty.");
                return;
            }
            
            localizedStringGroup.Strings[0]?.RefreshString();
        }
        
        public static LocalizedString Duplicate(this LocalizedString localizedString)
        {
            LocalizedString duplicated = new(localizedString.TableReference, localizedString.TableEntryReference);
            foreach (string variableName in localizedString.Keys)
                duplicated.Add(variableName, localizedString[variableName]);
            return duplicated;
        }
    }
}
#endif
