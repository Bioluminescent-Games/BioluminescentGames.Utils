#if PRIME_TWEEN

using PrimeTween;
using UnityEngine;

namespace BioluminescentGames.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFader : MonoBehaviour
    {
        [SerializeField] private TweenSettings<float> tweenSettings;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            Tween.Alpha(_canvasGroup, tweenSettings).OnComplete(() => gameObject.SetActive(false));
        }
    }
}

#endif
