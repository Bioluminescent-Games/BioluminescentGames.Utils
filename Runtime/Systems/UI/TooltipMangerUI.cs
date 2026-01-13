using BioluminescentGames.Utils.Utilities;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BioluminescentGames.Utils.Systems.UI
{
    public class TooltipMangerUI : BioluminescentSingleton<TooltipMangerUI>
    {
        [SerializeField] private RectTransform tooltip;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private float fadeSpeed = 5.0f;

        private bool _visible;
        private bool _graphicVisible;

        public void SetTooltip(string title, string description, int delaySeconds = 1)
        {
            titleText.text = title;
            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
            descriptionText.text = description;
            descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(description));

            _visible = true;

            if (canvasGroup.alpha < 0.1f)
                TimeUtils.Instance.WaitForSecondsBeforeExecuting(delaySeconds, () =>
                {
                    if (!_visible)
                        return;

                    _graphicVisible = true;
                });
            else
                _graphicVisible = true;
        }

        public void ClearTooltip()
        {
            _visible = false;
            _graphicVisible = false;
        }

        public override void OnUpdate()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 clampedMousePos = new Vector2(Mathf.Clamp(mousePos.x, 0, Screen.width), Mathf.Clamp(mousePos.y, 0, Screen.height));
            tooltip.position = clampedMousePos;

            float currentScalar = Screen.width / 1920.0f;

            if (tooltip.position.x + tooltip.rect.width * currentScalar > Screen.width)
                tooltip.position = new Vector2(tooltip.position.x - tooltip.rect.width * currentScalar, tooltip.position.y);

            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            int targetAlpha = _graphicVisible ? 1 : 0;

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }
}
