#region

using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif

#endregion

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    /// <summary>
    /// Abstract base class for settings, inheriting from ScriptableObject and implementing ISetting.
    /// </summary>
    [AutoStaticsCleanup]
    public abstract partial class Setting : ScriptableObject, ISetting
    {
        /// <summary>
        /// Gets the unique identifier for the setting.
        /// </summary>
        [field: SerializeField] public string ID { get; private set; }

        /// <summary>
        /// Gets the name of the setting as it appears in the menu.
        /// </summary>
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public LocalizedString NameInMenu { get; private set; }
#else
        [field: SerializeField] public string NameInMenu { get; private set; }
#endif

        /// <summary>
        /// Gets the category name of the setting.
        /// </summary>
        [field: SerializeField] public CategoryDefinition Category { get; private set; }

        /// <summary>
        /// Gets the order index of the setting.
        /// </summary>
        [field: SerializeField, HideInInspector] public int OrderIndex { get; private set; }

        /// <summary>
        /// The description of the tooltip for the setting.
        /// </summary>
#if BG_ENABLE_LOCALIZATION
        [field: SerializeField] public LocalizedString Description { get; private set; }
#else
        [field: SerializeField] public string Description { get; private set; }
#endif
        /// <summary>
        /// Initializes the setting. Can be overridden by derived classes.
        /// </summary>
        public virtual void Initialize() {}

        /// <summary>
        /// Applies the setting. Can be overridden by derived classes.
        /// </summary>
        public virtual void OnApply() {}

        /// <summary>
        /// Discard the value that would've been applied by calling OnApply.
        /// </summary>
        public virtual void DiscardValue() {}

#if UNITY_EDITOR
        public void EDITOR_SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;

        public void EDITOR_SetCategory(CategoryDefinition category) => Category = category;
#endif
    }
}
