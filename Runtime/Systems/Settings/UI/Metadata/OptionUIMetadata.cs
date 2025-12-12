using System;
using TMPro;
using UnityEngine;

namespace BackroomsGame.Systems.Settings.UI.Metadata
{
    public abstract class OptionUIMetadata : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Title { get; private set; }

        public event Action OnDirty;
        
        protected void SetDirty() => OnDirty?.Invoke();

        private void OnDestroy()
        {
            OnDirty = null;
        }

    }
}