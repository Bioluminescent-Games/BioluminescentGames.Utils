using BioluminescentGames.Systems.UpdateSystem;
using BioluminescentGames.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BackroomsGame.Systems.UI
{
    public class UITooltip : BioluminescentBehaviour
    {
        public string title;
        public string description;

        private bool _visible;

        public override void OnUpdate()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (RectTransformUtility.RectangleContainsScreenPoint(transform.AsRectTransform(), mousePos))
                ShowTooltip();
            else
                HideTooltip();
        }

        private void OnDisable()
        {
            HideTooltip();
        }

        public void ShowTooltip()
        {
            if (_visible)
                return;

            _visible = true;
            TooltipMangerUI.Instance.SetTooltip(title, description);
        }

        public void HideTooltip()
        {
            if (!_visible)
                return;

            _visible = false;
            TooltipMangerUI.Instance.ClearTooltip();
        }
    }
}
