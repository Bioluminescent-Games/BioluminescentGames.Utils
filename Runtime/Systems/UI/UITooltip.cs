using BioluminescentGames.Utils.Utilities;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

#if BG_ENABLE_LOCALIZATION
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
using UnityEngine.Localization;
#endif

#if UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace BioluminescentGames.Utils.Systems.UI
{
    public class UITooltip : BioluminescentBehaviour
    {
#if BG_ENABLE_LOCALIZATION
        [SerializeField] private bool useLocalization;
#endif
        
#if BG_ENABLE_LOCALIZATION && EDITOR_ATTRIBUTES
        [DisableField(nameof(useLocalization))]
#endif
        [SerializeField] private string title;
        
#if BG_ENABLE_LOCALIZATION && EDITOR_ATTRIBUTES
        [DisableField(nameof(useLocalization))]
#endif
        [SerializeField] private string description;

#if BG_ENABLE_LOCALIZATION
#if EDITOR_ATTRIBUTES
        [EnableField(nameof(useLocalization))]
#endif
        [SerializeField] private LocalizedString localizedTitle;
        
#if EDITOR_ATTRIBUTES
        [EnableField(nameof(useLocalization))]
#endif
        [SerializeField] private LocalizedString localizedDescription;
#endif

        private bool _visible;

        public override void OnUpdate()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (RectTransformUtility.RectangleContainsScreenPoint(transform.AsRectTransform(), mousePos))
                ShowTooltip();
            else
                HideTooltip();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            HideTooltip();
        }

        public void ShowTooltip()
        {
            if (_visible)
                return;
            
            _visible = true;

            UpdateTooltip().Forget();
        }

#if UNITASK
        private async UniTaskVoid UpdateTooltip()
#else
        private async Task UpdateTooltip()
#endif
        {
            if (!_visible)
                return;

            string titleToShow = title;
            string descriptionToShow = description;
            
#if BG_ENABLE_LOCALIZATION
            if (useLocalization)
            {
#if UNITASK
                titleToShow = await localizedTitle.GetLocalizedStringAsync();
                descriptionToShow = await localizedDescription.GetLocalizedStringAsync();
#else
                titleToShow = await localizedTitle.GetLocalizedStringAsync().Task;
                descriptionToShow = await localizedDescription.GetLocalizedStringAsync().Task;
#endif
            }
#endif

            if (string.IsNullOrWhiteSpace(titleToShow) && string.IsNullOrWhiteSpace(descriptionToShow))
            {
                _visible = false;
                return;
            }

            TooltipMangerUI.Instance.SetTooltip(titleToShow, descriptionToShow);
        }

        public void HideTooltip()
        {
            if (!_visible)
                return;

            _visible = false;
            TooltipMangerUI.Instance.ClearTooltip();
        }

        public void SetTooltip(string newTitle, string newDescription)
        {
            title = newTitle;
            description = newDescription;
            
#if BG_ENABLE_LOCALIZATION
            useLocalization = false;
            if (localizedTitle != null)
                localizedTitle.StringChanged -= LocalizedTitleOnStringChanged;
            
            if (localizedDescription != null)
                localizedDescription.StringChanged -= LocalizedDescriptionOnStringChanged;
#endif

            UpdateTooltip().Forget();
        }

#if BG_ENABLE_LOCALIZATION
        public void SetTooltip(LocalizedString newTitle, LocalizedString newDescription)
        {
            if (localizedTitle != null)
                localizedTitle.StringChanged -= LocalizedTitleOnStringChanged;
            
            if (localizedDescription != null)
                localizedDescription.StringChanged -= LocalizedDescriptionOnStringChanged;
            
            useLocalization = true;
            
            localizedTitle = newTitle;
            localizedDescription = newDescription;
            
            localizedTitle.StringChanged += LocalizedTitleOnStringChanged;
            localizedDescription.StringChanged += LocalizedDescriptionOnStringChanged;

            UpdateTooltip().Forget();
        }
        
        private void LocalizedTitleOnStringChanged(string value)
        {
            UpdateTooltip().Forget();
        }

        private void LocalizedDescriptionOnStringChanged(string value)
        {
            UpdateTooltip().Forget();
        }
#endif
    }
}
