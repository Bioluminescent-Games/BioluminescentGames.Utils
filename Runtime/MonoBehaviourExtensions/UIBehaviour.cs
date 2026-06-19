#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif

#if PRIMETWEEN
using PrimeTween;
#endif

using UnityEngine;

namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    /// <summary>
    /// Base UI class contains common methods used for UI.
    /// </summary>
    public abstract class UIBehaviour : MonoBehaviour
    {
#if PRIMETWEEN
        private const float k_DefaultDuration = 0.2f;

        [Header("UI Behaviour")]
        [SerializeField] protected bool shouldAnimate = true;

        [Space(2)]
        [SerializeField] protected CanvasGroup darkenCanvasGroup;
        
        [SerializeField] protected TweenSettings<float> darkenTweenSettings = new(0.0f, 1.0f, k_DefaultDuration, Ease.InSine);
        [SerializeField] protected TweenSettings<float> brightenTweenSettings = new(1.0f, 0.0f, k_DefaultDuration, Ease.OutSine);

        [Space(2)]
        [SerializeField] protected RectTransform[] containers;
        
        [SerializeField] protected TweenSettings<float> scaleInTweenSettings = new(0.0f, 1.0f, k_DefaultDuration, Ease.InSine);
        [SerializeField] protected TweenSettings<float> scaleOutTweenSettings = new(1.0f, 0.0f, k_DefaultDuration, Ease.OutSine);

#if EDITOR_ATTRIBUTES
        [Space(4)]
        [Header("Custom Fields")]
        [SerializeField] private Void _;
#endif

        private Sequence _currentTween;

        /// <summary>
        /// Animate the UI showing/hiding.
        /// NOTE: This does NOT change the GameObject's active state!
        /// </summary>
        /// <param name="showing">Should we play the show or the hide animation?</param>
        /// <returns>The sequence containing the Tween.</returns>
        protected virtual Sequence Animate(bool showing)
        {
            bool unscaledTime = (showing ? darkenTweenSettings : brightenTweenSettings).settings.useUnscaledTime ||
                                (showing ? scaleInTweenSettings : scaleOutTweenSettings).settings.useUnscaledTime;
            Sequence sequence = Sequence.Create(useUnscaledTime: unscaledTime);

            if (darkenCanvasGroup)
                sequence.Group(Tween.Alpha(darkenCanvasGroup, showing ? darkenTweenSettings : brightenTweenSettings));

            foreach (RectTransform container in containers)
                if (container)
                    sequence.Group(Tween.Scale(container, showing ? scaleInTweenSettings : scaleOutTweenSettings));

            return sequence;
        }
#endif

        /// <summary>
        /// Shows the UI
        /// </summary>
        protected virtual void Show(bool animate)
        {
            if (gameObject.activeSelf) return; // Bail out if already visible

#if PRIMETWEEN
            _currentTween.Complete();
#endif

            gameObject.SetActive(true);

            OnShowing();
            OnVisibilityChanging(true);

#if PRIMETWEEN
            if (animate && shouldAnimate)
            {
                _currentTween = Animate(true);
                _currentTween.OnComplete(ShowComplete, false);
            }
            else
#endif
                ShowComplete();

            return;

            void ShowComplete()
            {
                OnShown();
                OnVisibilityChanged(true);
            }
        }

        /// <summary>
        /// Shows the UI with animation
        /// </summary>
        protected virtual void Show() => Show(true);

        /// <summary>
        /// OnShowing is called when the UI is showing.
        /// </summary>
        protected virtual void OnShowing() {}

        /// <summary>
        /// OnShown is called when the UI is shown.
        /// </summary>
        protected virtual void OnShown() {}

        /// <summary>
        /// Hides the UI
        /// </summary>
        protected virtual void Hide(bool animate)
        {
            if (!gameObject.activeSelf) return; // Bail out if already hidden

#if PRIMETWEEN
            _currentTween.Complete();
#endif

            OnHiding();
            OnVisibilityChanging(false);

#if PRIMETWEEN
            if (animate && shouldAnimate)
            {
                _currentTween = Animate(false);
                _currentTween.OnComplete(HideComplete, false);
            }
            else
#endif
                HideComplete();

            return;

            void HideComplete()
            {
                OnHidden();
                OnVisibilityChanged(false);

                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Hides the UI with animation
        /// </summary>
        protected virtual void Hide() => Hide(true);

        /// <summary>
        /// OnHiding is called when the UI is hiding.
        /// </summary>
        protected virtual void OnHiding() {}

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
        /// <param name="animate">Should the UI play the tween when changing the visibility?</param>
        protected virtual void SetVisibility(bool visible, bool animate = true)
        {
            if (visible)
                Show(animate);
            else
                Hide(animate);
        }

        /// <summary>
        /// Called when the visibility of the UI changes.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanged(bool newVisibility) {}

        /// <summary>
        /// Called when the visibility of the UI is changing.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanging(bool newVisibility) {}

        /// <summary>
        /// Toggle the visibility of the current object.
        /// </summary>
        protected virtual void ToggleVisibility(bool animate = true) => SetVisibility(!IsVisible(), animate);

        protected virtual void OnDestroy()
        {
#if PRIMETWEEN
            _currentTween.Complete();
#endif
        }
    }

    /// <summary>
    /// Base UI class contains common methods used for UI. But Bioluminescerized.
    /// </summary>
    public abstract class BioluminescentUIBehaviour : BioluminescentBehaviour
    {
#if PRIMETWEEN
        private const float k_DefaultDuration = 0.2f;

        [Header("UI Behaviour")]
        [SerializeField] protected bool shouldAnimate = true;

        [Space(2)]
        [SerializeField] protected CanvasGroup darkenCanvasGroup;
        
        [SerializeField] protected TweenSettings<float> darkenTweenSettings = new(0.0f, 1.0f, k_DefaultDuration, Ease.InSine);
        [SerializeField] protected TweenSettings<float> brightenTweenSettings = new(1.0f, 0.0f, k_DefaultDuration, Ease.OutSine);

        [Space(2)]
        [SerializeField] protected RectTransform[] containers;
        
        [SerializeField] protected TweenSettings<float> scaleInTweenSettings = new(0.0f, 1.0f, k_DefaultDuration, Ease.InSine);
        [SerializeField] protected TweenSettings<float> scaleOutTweenSettings = new(1.0f, 0.0f, k_DefaultDuration, Ease.OutSine);

#if EDITOR_ATTRIBUTES
        [Space(4)]
        [Header("Custom Fields")]
        [SerializeField] private Void _;
#endif

        private Sequence _currentTween;

        /// <summary>
        /// Animate the UI showing/hiding.
        /// NOTE: This does NOT change the GameObject's active state!
        /// </summary>
        /// <param name="showing">Should we play the show or the hide animation?</param>
        /// <returns>The sequence containing the Tween.</returns>
        protected virtual Sequence Animate(bool showing)
        {
            bool unscaledTime = (showing ? darkenTweenSettings : brightenTweenSettings).settings.useUnscaledTime ||
                                (showing ? scaleInTweenSettings : scaleOutTweenSettings).settings.useUnscaledTime;
            Sequence sequence = Sequence.Create(useUnscaledTime: unscaledTime);

            if (darkenCanvasGroup)
                sequence.Group(Tween.Alpha(darkenCanvasGroup, showing ? darkenTweenSettings : brightenTweenSettings));

            foreach (RectTransform container in containers)
                if (container)
                    sequence.Group(Tween.Scale(container, showing ? scaleInTweenSettings : scaleOutTweenSettings));

            return sequence;
        }
#endif

        /// <summary>
        /// Shows the UI
        /// </summary>
        protected virtual void Show(bool animate)
        {
            if (gameObject.activeSelf) return; // Bail out if already visible

#if PRIMETWEEN
            _currentTween.Complete();
#endif

            gameObject.SetActive(true);

            OnShowing();
            OnVisibilityChanging(true);

#if PRIMETWEEN
            if (animate && shouldAnimate)
            {
                _currentTween = Animate(true);
                _currentTween.OnComplete(ShowComplete, false);
            }
            else
#endif
                ShowComplete();

            return;

            void ShowComplete()
            {
                OnShown();
                OnVisibilityChanged(true);
            }
        }

        /// <summary>
        /// Shows the UI with animation
        /// </summary>
        protected virtual void Show() => Show(true);

        /// <summary>
        /// OnShowing is called when the UI is showing.
        /// </summary>
        protected virtual void OnShowing() {}

        /// <summary>
        /// OnShown is called when the UI is shown.
        /// </summary>
        protected virtual void OnShown() {}

        /// <summary>
        /// Hides the UI
        /// </summary>
        protected virtual void Hide(bool animate)
        {
            if (!gameObject.activeSelf) return; // Bail out if already hidden

#if PRIMETWEEN
            _currentTween.Complete();
#endif

            OnHiding();
            OnVisibilityChanging(false);

#if PRIMETWEEN
            if (animate && shouldAnimate)
            {
                _currentTween = Animate(false);
                _currentTween.OnComplete(HideComplete, false);
            }
            else
#endif
                HideComplete();

            return;

            void HideComplete()
            {
                OnHidden();
                OnVisibilityChanged(false);

                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Hides the UI with animation
        /// </summary>
        protected virtual void Hide() => Hide(true);

        /// <summary>
        /// OnHiding is called when the UI is hiding.
        /// </summary>
        protected virtual void OnHiding() {}

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
        /// <param name="animate">Should the UI play the tween when changing the visibility?</param>
        protected virtual void SetVisibility(bool visible, bool animate = true)
        {
            if (visible)
                Show(animate);
            else
                Hide(animate);
        }

        /// <summary>
        /// Called when the visibility of the UI changes.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanged(bool newVisibility) {}

        /// <summary>
        /// Called when the visibility of the UI is changing.
        /// </summary>
        /// <param name="newVisibility">The new visibility state of the UI.</param>
        protected virtual void OnVisibilityChanging(bool newVisibility) {}

        /// <summary>
        /// Toggle the visibility of the current object.
        /// </summary>
        protected virtual void ToggleVisibility(bool animate = true) => SetVisibility(!IsVisible(), animate);

        protected virtual void OnDestroy()
        {
#if PRIMETWEEN
            _currentTween.Complete();
#endif
        }
    }
}
