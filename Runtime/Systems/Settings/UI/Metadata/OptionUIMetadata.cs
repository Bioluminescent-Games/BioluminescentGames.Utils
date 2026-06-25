using System;
using TMPro;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class OptionUIMetadata : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Title { get; private set; }

        public event Action Dirty;

        protected void SetDirty() => Dirty?.Invoke();

        private void OnDestroy()
        {
            Dirty = null;
        }

    }
}
