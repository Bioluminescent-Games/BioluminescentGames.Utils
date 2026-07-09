using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace BioluminescentGames.Utils.Systems.Settings
{
    public interface ISetting
    {
        public string ID { get; }
#if BG_ENABLE_LOCALIZATION
        public LocalizedString NameInMenu { get; }
        public LocalizedString Description { get; }
#else
        public string NameInMenu { get; }
        public string Description { get; }
#endif
        public CategoryDefinition Category { get; }
        public int OrderIndex { get; }

        void Initialize();
        void OnApply();
        void DiscardValue();
    }
}
