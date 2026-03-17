namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    public abstract class SavableSetting : Setting
    {
        public override void Init()
        {
            LoadFromPlayerPrefs();
        }

        public override void OnApply()
        {
            SaveToPlayerPrefs();
        }

        protected abstract void LoadFromPlayerPrefs();
        protected abstract void SaveToPlayerPrefs();
    }
}
