using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BioluminescentGames.Utils.Systems.Settings.UI.Metadata
{
    public class FloatOptionUIMetadata : OptionUIMetadata
    {
        [field: SerializeField] public Slider Slider { get; private set; }
        [SerializeField] private TMP_InputField sliderValue;

        private void Awake()
        {
            Slider.onValueChanged.AddListener(_ => SetDirty());
            Slider.onValueChanged.AddListener(UpdateText);
            sliderValue.onValueChanged.AddListener(newVal => Slider.value = float.Parse(newVal, CultureInfo.InvariantCulture));
            
            UpdateText(Slider.value);
        }

        private void UpdateText(float newVal)
        {
            sliderValue.text = (Mathf.Round(newVal * 10.0f) / 10.0f).ToString(CultureInfo.InvariantCulture);
        }
    }
}