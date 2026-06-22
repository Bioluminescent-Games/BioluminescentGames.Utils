namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    public abstract class SavableSetting : Setting
    {
        /// <summary>
        /// Additional string to append to the ID when saving to PlayerPrefs.
        /// Useful for saving different setting values for different contexts, such as resolution for different displays.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// The ID used for saving.
        /// </summary>
        public string IDForSaving => $"{ID}_{Variant}";

        public override void Initialize()
        {
            Reload();
        }

        public virtual void Reload()
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
