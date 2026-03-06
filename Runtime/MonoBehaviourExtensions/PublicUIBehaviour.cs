namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    public interface IPublicUIBehaviour
    {
        public void ShowObject(bool animate);
        public void HideObject(bool animate);
        public void ToggleObjectVisibility(bool animate);
        public void SetObjectVisibility(bool visibility, bool animate);

        public void ShowObject();
        public void HideObject();
        public void ToggleObjectVisibility();
        public void SetObjectVisibility(bool visibility);

        public bool IsObjectVisible();
        public bool IsObjectInVisible();
    }

    public class PublicUIBehaviour : UIBehaviour, IPublicUIBehaviour
    {
        public void ShowObject(bool animate) => Show(animate);
        public void HideObject(bool animate) => Hide(animate);
        public void ToggleObjectVisibility(bool animate) => ToggleVisibility(animate);
        public void SetObjectVisibility(bool visibility, bool animate) => SetVisibility(visibility, animate);

        public void ShowObject() => Show();
        public void HideObject() => Hide();
        public void ToggleObjectVisibility() => ToggleVisibility();
        public void SetObjectVisibility(bool visibility) => SetVisibility(visibility);

        public bool IsObjectVisible() => IsVisible();
        public bool IsObjectInVisible() => IsInvisible();
    }

    public class BioluminescentPublicUIBehaviour : BioluminescentUIBehaviour, IPublicUIBehaviour
    {
        public void ShowObject(bool animate) => Show(animate);
        public void HideObject(bool animate) => Hide(animate);
        public void ToggleObjectVisibility(bool animate) => ToggleVisibility(animate);
        public void SetObjectVisibility(bool visibility, bool animate) => SetVisibility(visibility, animate);

        public void ShowObject() => Show();
        public void HideObject() => Hide();
        public void ToggleObjectVisibility() => ToggleVisibility();
        public void SetObjectVisibility(bool visibility) => SetVisibility(visibility);

        public bool IsObjectVisible() => IsVisible();
        public bool IsObjectInVisible() => IsInvisible();
    }
}
