namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    public class PublicUIBehaviour : UIBehaviour
    {
        public void ShowObject(bool animate) => Show(animate);
        public void HideObject(bool animate) => Hide(animate);

        public void ShowObject() => Show();
        public void HideObject() => Hide();
    }

    public class BioluminescentPublicUIBehaviour : BioluminescentUIBehaviour
    {
        public void ShowObject(bool animate) => Show(animate);
        public void HideObject(bool animate) => Hide(animate);

        public void ShowObject() => Show();
        public void HideObject() => Hide();
    }
}
