using System;
using UnityEngine;
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization.Components;
#else
using TMPro;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class OptionUIMetadata : MonoBehaviour
    {
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public LocalizeStringEvent Title { get; private set; }
#else
        [field: SerializeField] public TMP_Text Title { get; private set; }
#endif

        public event Action Dirty;

        protected void SetDirty() => Dirty?.Invoke();

        private void OnDestroy()
        {
            Dirty = null;
        }

    }
}
