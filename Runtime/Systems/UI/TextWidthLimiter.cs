using BioluminescentGames.Utils.MonoBehaviourExtensions;
using TMPro;
using UnityEngine;

namespace BackroomsGame.Systems.UI
{
    [RequireComponent(typeof(TMP_Text))]
    [ExecuteAlways]
    public class TextWidthLimiter : BioluminescentBehaviour
    {
        [SerializeField] private float maxWidth = 800f;

        private TMP_Text _text;

        protected override void Awake()
        {
            base.Awake();

            _text = GetComponent<TMP_Text>();
        }

        public override void OnUpdate()
        {
            bool exceeds = _text.preferredWidth > maxWidth;
            _text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                exceeds ? maxWidth : _text.preferredWidth);
        }
    }
}
