namespace BioluminescentGames.Utils.Utilities
{
    public class PublicUIBehaviour : UIBehaviour
    {
        public void ShowObject() => Show();
        public void HideObject() => Hide();
    }

    public class BioluminescentPublicUIBehaviour : BioluminescentUIBehaviour
    {
        public void ShowObject() => Show();
        public void HideObject() => Hide();
    }
}
