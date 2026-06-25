using System.Collections.Generic;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using BioluminescentGames.Utils.Utilities;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZLinq;

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

        [Header("Animations")]
        [SerializeField] private TweenSettings<float> previousTween;
        [SerializeField] private TweenSettings<float> nextTween;
        
        [SerializeField] private TweenSettings<float> noNextTween;
        [SerializeField] private TweenSettings<float> noPrevTween;
        [SerializeField] private TweenSettings<float> neutralTween;
        
        [Header("Dots")]
        [SerializeField] private Image dotTemplate;
        [SerializeField] private Transform dotsParent;
        [SerializeField] private Color deselectedColor = Color.white;
        [SerializeField] private Color selectedColor = Color.gray;

        [Header("Options")]
        [SerializeField] public List<string> options = new();
        
        [SerializeField] public UnityEvent<int> onValueChanged = new();
        
        public int Value { get; private set; }

        private List<string> _oldOptions = new();

        private Sequence _currentTween;
        
        private Sequence _nextTween;

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
            if (!_oldOptions.AsValueEnumerable().SequenceEqual(options))
            {
                UpdateDots();
                UpdateTexts();
                _oldOptions = options;
            }
        }

        private void Prev()
        {
            ChangeValueAndPlayAnimations(-1, previousTween);
        }

        private void Next()
        {
            ChangeValueAndPlayAnimations(+1, nextTween);
        }

        private void ChangeValueAndPlayAnimations(int delta, TweenSettings<float> animation)
        {
            if (Value <= 0 && delta < 0)
            {
                neutralTween.startValue = noPrevTween.endValue;
                PlaySequence(Tween.UIAnchoredPositionX(textsParent, noPrevTween)
                    .Chain(Tween.UIAnchoredPositionX(textsParent, neutralTween)));
                return;
            }

            if (Value >= options.Count - 1 && delta > 0)
            {
                neutralTween.startValue = noNextTween.endValue;
                PlaySequence(Tween.UIAnchoredPositionX(textsParent, noNextTween)
                    .Chain(Tween.UIAnchoredPositionX(textsParent, neutralTween)));
                return;
            }

            Value += delta;
            onValueChanged.Invoke(Value);
            PlaySequence(Sequence.Create(Tween.UIAnchoredPositionX(textsParent, animation)).OnComplete(UpdateTexts));
        }

        private void PlaySequence(Sequence sequence)
        {
            _currentTween.Complete();
            _currentTween = sequence;
        }

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
            Value = value;
            onValueChanged.Invoke(Value);
            UpdateTexts();
        }

        public void SetValueNoNotify(int value)
        {
            Value = value;
            UpdateTexts();
            UpdateDots();
        }
    }
}
