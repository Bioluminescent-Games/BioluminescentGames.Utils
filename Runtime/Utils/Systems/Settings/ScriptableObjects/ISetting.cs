using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;

namespace BioluminescentGames.Utils.Systems.Settings
{
    public interface ISetting
    {
        public string ID { get; }
        public string NameInMenu { get; }
        public CategoryDefinition Category { get; }
        public int OrderIndex { get; }
        public string TooltipDescription { get; }

        void Init();
        void OnApply();
    }
}
