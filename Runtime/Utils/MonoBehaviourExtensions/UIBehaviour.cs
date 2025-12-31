using BioluminescentGames.Utils.MonoBehaviourExtensions;
using UnityEngine;

namespace BioluminescentGames.Utils
{
    /// <summary>
    /// Base UI class, contains common methods used for UI.
    /// </summary>
    public abstract class UIBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Shows the UI
        /// </summary>
        protected virtual void Show()
        {
            if (gameObject.activeSelf) return; // Bail out if already visible

            gameObject.SetActive(true);
            OnShown();
            OnVisibilityChanged(true);
        }

        /// <summary>
        /// OnShown is called when the UI is shown.
        /// </summary>
        protected virtual void OnShown() {}

        /// <summary>
        /// Hides the UI
        /// </summary>
        protected virtual void Hide()
        {
            if (!gameObject.activeSelf) return; // Bail out if already hidden

            gameObject.SetActive(false);
            OnHidden();
            OnVisibilityChanged(false);
        }

        /// <summary>
        /// OnHidden is called when the UI is hidden.
        /// </summary>
        protected virtual void OnHidden() {}

        /// <summary>
        /// Is the UI Object visible?
        /// </summary>
        protected virtual bool IsVisible() => gameObject.activeSelf;

        /// <summary>
        /// Is the UI Object not visible?
        /// </summary>
        protected virtual bool IsInvisible() => !IsVisible();

        /// <summary>
        /// Sets the UI's visibility.
        /// </summary>
        /// <param name="visible">If the UI should be visible</param>
        protected virtual void SetVisibility(bool visible)
        {
            if (visible)
                Show();
            else
                Hide();
        }

        /// <summary>
        /// Called when the visibility of the UI changes.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanged(bool newVisibility) {}

        /// <summary>
        /// Toggle the visibility of the current object.
        /// </summary>
        protected virtual void ToggleVisibility() => SetVisibility(!IsVisible());
    }

    /// <summary>
    /// Base UI class, contains common methods used for UI.
    /// </summary>
    public abstract class BioluminescentUIBehaviour : BioluminescentBehaviour
    {
        /// <summary>
        /// Shows the UI
        /// </summary>
        protected virtual void Show()
        {
            if (gameObject.activeSelf) return; // Bail out if already visible

            gameObject.SetActive(true);
            OnShown();
            OnVisibilityChanged(true);
        }

        /// <summary>
        /// OnShown is called when the UI is shown.
        /// </summary>
        protected virtual void OnShown() {}

        /// <summary>
        /// Hides the UI
        /// </summary>
        protected virtual void Hide()
        {
            if (!gameObject.activeSelf) return; // Bail out if already hidden

            gameObject.SetActive(false);
            OnHidden();
            OnVisibilityChanged(false);
        }

        /// <summary>
        /// OnHidden is called when the UI is hidden.
        /// </summary>
        protected virtual void OnHidden() {}

        /// <summary>
        /// Is the UI Object visible?
        /// </summary>
        protected virtual bool IsVisible() => gameObject.activeSelf;

        /// <summary>
        /// Is the UI Object not visible?
        /// </summary>
        protected virtual bool IsInvisible() => !IsVisible();

        /// <summary>
        /// Sets the UI's visibility.
        /// </summary>
        /// <param name="visible">If the UI should be visible</param>
        protected virtual void SetVisibility(bool visible)
        {
            if (visible)
                Show();
            else
                Hide();
        }

        /// <summary>
        /// Called when the visibility of the UI changes.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanged(bool newVisibility) {}

        /// <summary>
        /// Toggle the visibility of the current object.
        /// </summary>
        protected virtual void ToggleVisibility() => SetVisibility(!IsVisible());
    }
}
