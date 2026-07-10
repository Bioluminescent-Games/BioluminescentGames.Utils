#region

using System;
using UnityEngine;
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif

#endregion

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Settings Category", menuName = "Scriptable Objects/Settings/Category")]
    public class CategoryDefinition : ScriptableObject, IEquatable<CategoryDefinition>, IComparable<CategoryDefinition>
    {
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public bool LocalizeDisplayName { get; private set; } = true;
        
#if EDITOR_ATTRIBUTES
        [EnableField(nameof(LocalizeDisplayName))]
#endif
        [field: SerializeField] public LocalizedString LocalizedDisplayName { get; private set; }
        
#if EDITOR_ATTRIBUTES
        [DisableField(nameof(LocalizeDisplayName))]
#endif
#endif
        [field: SerializeField] public string DisplayName { get; private set; }
        
        [field: SerializeField] public int OrderIndex { get; private set; }

#if UNITY_EDITOR
        public void EDITOR_SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
#endif

        public bool Equals(CategoryDefinition other)
        {
            return UnityEngine.Object.Equals(this, other);
        }

        public int CompareTo(CategoryDefinition other)
        {
            return OrderIndex.CompareTo(other.OrderIndex);
        }
    }
}
