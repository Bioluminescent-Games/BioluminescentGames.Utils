using System.Collections.Generic;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using BioluminescentGames.Utils.Utilities;
#if PRIMETWEEN
using PrimeTween;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BackroomsGame.UI
{
    public class HorizontalMultiChoice : BioluminescentBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        
        [Header("Texts")]
        [SerializeField] private RectTransform textsParent;
        [SerializeField] private TMP_Text currentText;
        [SerializeField] private TMP_Text previousText;
        [SerializeField] private TMP_Text nextText;

#if PRIMETWEEN
        [Header("Animations")]
        [SerializeField] private TweenSettings<float> previousTween;
        [SerializeField] private TweenSettings<float> nextTween;
        
        [SerializeField] private TweenSettings<float> noNextTween;
        [SerializeField] private TweenSettings<float> noPrevTween;
        [SerializeField] private TweenSettings<float> neutralTween;
#endif
        
        [Header("Dots")]
        [SerializeField] private Image dotTemplate;
        [SerializeField] private Transform dotsParent;
        [SerializeField] private Color deselectedColor = Color.white;
        [SerializeField] private Color selectedColor = Color.gray;

        [Header("Options")]
        [SerializeField] public List<string> options = new();
        
        [SerializeField] public UnityEvent<int> onValueChanged = new();
        
        public int Value { get; private set; }

        private string[] _oldOptions;

#if PRIMETWEEN
        private Sequence _currentTween;
        private Sequence _nextTween;
#endif

        private void Start()
        {
            nextButton.onClick.AddListener(Next);
            prevButton.onClick.AddListener(Prev);

            UpdateDots();
            UpdateTexts();
            
            onValueChanged.AddListener(_ => UpdateDots());
        }

        public override void OnUpdate()
        {
            _oldOptions ??= options.ToArray();

            Value = Mathf.Clamp(Value, 0, options.Count - 1);
            
            if (!_oldOptions
#if ZLINQ
                    .AsValueEnumerable()
#endif
                    .SequenceEqual(options))
            {
                UpdateDots();
                UpdateTexts();
                _oldOptions = options.ToArray();
            }
        }

        private void Prev()
        {
#if PRIMETWEEN
            ChangeValueAndPlayAnimations(-1, previousTween);
#else
            ChangeValueAndPlayAnimations(-1);
#endif
        }

        private void Next()
        {
#if PRIMETWEEN
            ChangeValueAndPlayAnimations(+1, nextTween);
#else
            ChangeValueAndPlayAnimations(+1);
#endif
        }

#if PRIMETWEEN
        private void ChangeValueAndPlayAnimations(int delta, TweenSettings<float> animation)
#else
        private void ChangeValueAndPlayAnimations(int delta)
#endif
        {
            if (Value <= 0 && delta < 0)
            {
#if PRIMETWEEN
                neutralTween.startValue = noPrevTween.endValue;
                PlaySequence(Sequence.Create(useUnscaledTime: noPrevTween.settings.useUnscaledTime)
                    .Group(Tween.UIAnchoredPositionX(textsParent, noPrevTween))
                    .Chain(Tween.UIAnchoredPositionX(textsParent, neutralTween)));
#endif
                return;
            }

            if (Value >= options.Count - 1 && delta > 0)
            {
#if PRIMETWEEN
                neutralTween.startValue = noNextTween.endValue;
                PlaySequence(Sequence.Create(useUnscaledTime: noNextTween.settings.useUnscaledTime)
                    .Group(Tween.UIAnchoredPositionX(textsParent, noNextTween))
                    .Chain(Tween.UIAnchoredPositionX(textsParent, neutralTween)));
#endif
                return;
            }

            Value += delta;
            onValueChanged.Invoke(Value);
#if PRIMETWEEN
            PlaySequence(Sequence.Create(useUnscaledTime: animation.settings.useUnscaledTime)
                .Group(Tween.UIAnchoredPositionX(textsParent, animation)).OnComplete(UpdateTexts));
#else
            UpdateTexts();
#endif
        }

#if PRIMETWEEN
        private void PlaySequence(Sequence sequence)
        {
            _currentTween.Complete();
            _currentTween = sequence;
        }
#endif

        private void UpdateDots()
        {
            dotsParent.ClearChildren(t => t.gameObject.activeSelf); // Template isn't active.

            for (int i = 0; i < options.Count; i++)
            {
                Image dot = Instantiate(dotTemplate, dotsParent);
                dot.color = i == Value ? selectedColor : deselectedColor;
                dot.gameObject.SetActive(true);
            }
        }

        private void UpdateTexts()
        {
            currentText.text = options[Value];
            
            previousText.text = Value > 0 
                ? options[Value - 1] 
                : string.Empty;
            
            nextText.text = Value < options.Count - 1 
                ? options[Value + 1] 
                : string.Empty;

            textsParent.anchoredPosition = Vector2.zero;
        }

        public void SetValue(int value)
        {
            Value = Mathf.Clamp(value, 0, options.Count - 1);
            onValueChanged.Invoke(Value);
            UpdateTexts();
        }

        public void SetValueNoNotify(int value)
        {
            Value = Mathf.Clamp(value, 0, options.Count - 1);
            UpdateTexts();
            UpdateDots();
        }

        public void ClearOptions()
        {
            options.Clear();
        }

        public void AddOptions(List<string> newOptions)
        {
            options.AddRange(newOptions);
        }
    }
}
