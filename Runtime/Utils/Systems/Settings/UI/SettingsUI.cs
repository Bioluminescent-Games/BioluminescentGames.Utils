using System;
using System.Collections.Generic;
using ZLinq;
using BackroomsGame.Systems.Settings.ScriptableObjects;
using BackroomsGame.Systems.Settings.UI.Metadata;
using BackroomsGame.Systems.UI;
using BioluminescentGames.Utils.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackroomsGame.Systems.Settings
{
    public class SettingsUI : UIBehaviour
    {
        [Header("Generic")]
        [SerializeField] private Button applyButton;

        [Header("Categories")]
        [SerializeField] private Transform categoryParent;
        [SerializeField] private Button categoryPrefab;

        [Header("Settings")]
        [Space(6)]
        [SerializeField] private Transform settingsParent;

        [Space(4)]
        [SerializeField] private CheckboxOptionUIMetadata checkboxPrefab;
        [SerializeField] private IntOptionUIMetadata intPrefab;
        [SerializeField] private FloatOptionUIMetadata floatPrefab;
        [SerializeField] private DropdownOptionUIMetadata dropdownPrefab;
        [SerializeField] private ButtonOptionUIMetadata buttonPrefab;

        private readonly HashSet<string> _settingsModified = new HashSet<string>();

        private void Awake()
        {
            applyButton.onClick.AddListener(() =>
            {
                foreach (ISetting settingObject in _settingsModified.AsValueEnumerable().Select(Settings.Get))
                {
                    Debug.Log($"Settings > Apply: {settingObject.NameInMenu}");
                    settingObject.OnApply();
                }

                _settingsModified.Clear();

                Hide();
            });
        }

        private void Start()
        {
            Debug.Log("Settings > Loading Categories...");

            // Collect Categories
            List<string> categories = new List<string>();
            categories.AddRange(CollectCategories());
            categories = categories.AsValueEnumerable().OrderBy(category => category).ToList(); // Sort Alphabetically so it isn't random order.

            foreach (string category in categories)
            {
                Button categoryButton = Instantiate(categoryPrefab, categoryParent);
                categoryButton.GetComponentInChildren<TMP_Text>().text = category;
                categoryButton.onClick.AddListener(() => InstantiateSettingsInCategory(category));
            }

            InstantiateSettingsInCategory(categories[0]);
        }

        private void InstantiateSettingsInCategory(string category)
        {
            Debug.Log($"Settings > Instantiating Settings inside {category}...");

            settingsParent.ClearAllChildren();

            List<ISetting> settings = new List<ISetting>();
            settings.AddRange(GetSettingsInCategory(category));
            settings.Sort((setting1, setting2) => setting1.OrderIndex.CompareTo(setting2.OrderIndex));

            foreach (ISetting setting in settings)
            {
                UITooltip tooltip;

                switch (setting)
                {
                    case BoolSetting boolSetting:
                        CheckboxOptionUIMetadata checkboxOption = Instantiate(checkboxPrefab, settingsParent);
                        checkboxOption.Title.text = boolSetting.NameInMenu;
                        checkboxOption.Checkbox.isOn = boolSetting.Value;
                        checkboxOption.OnDirty += () => _settingsModified.Add(boolSetting.ID);
                        // ReSharper disable AccessToModifiedClosure
                        checkboxOption.OnDirty += () => boolSetting.Value = checkboxOption.Checkbox.isOn;

                        tooltip = checkboxOption.GetComponent<UITooltip>();
                        break;
                    case IntSetting intSetting:
                        IntOptionUIMetadata intOption = Instantiate(intPrefab, settingsParent);
                        intOption.Title.text = intSetting.NameInMenu;
                        intOption.Slider.maxValue = intSetting.MaxValue;
                        intOption.Slider.minValue = intSetting.MinValue;
                        intOption.Slider.value = intSetting.Value;
                        intOption.OnDirty += () => _settingsModified.Add(intSetting.ID);
                        intOption.OnDirty += () => intSetting.Value = Mathf.RoundToInt(intOption.Slider.value);

                        tooltip = intOption.GetComponent<UITooltip>();
                        break;
                    case FloatSetting floatSetting:
                        FloatOptionUIMetadata floatOption = Instantiate(floatPrefab, settingsParent);
                        floatOption.Title.text = floatSetting.NameInMenu;
                        floatOption.Slider.maxValue = floatSetting.MaxValue;
                        floatOption.Slider.minValue = floatSetting.MinValue;
                        floatOption.Slider.value = floatSetting.Value;
                        floatOption.OnDirty += () => _settingsModified.Add(floatSetting.ID);
                        floatOption.OnDirty += () => floatSetting.Value = floatOption.Slider.value;

                        tooltip = floatOption.GetComponent<UITooltip>();
                        break;
                    case DropdownSetting dropdownSetting:
                        DropdownOptionUIMetadata dropdownOption = Instantiate(dropdownPrefab, settingsParent);
                        dropdownOption.Title.text = dropdownSetting.NameInMenu;
                        dropdownOption.Dropdown.options = dropdownSetting.Options;
                        dropdownOption.Dropdown.value = (int)dropdownSetting.Value;
                        dropdownOption.OnDirty += () => _settingsModified.Add(dropdownSetting.ID);
                        dropdownOption.OnDirty += () => dropdownSetting.Value = (uint)dropdownOption.Dropdown.value;

                        tooltip = dropdownOption.GetComponent<UITooltip>();
                        // ReSharper restore AccessToModifiedClosure
                        break;
                    case ButtonSetting buttonSetting:
                        ButtonOptionUIMetadata buttonOption = Instantiate(buttonPrefab, settingsParent);
                        buttonOption.Title.text = buttonSetting.NameInMenu;
                        buttonOption.Button.onClick.AddListener(buttonSetting.ButtonPressed);

                        tooltip = buttonOption.GetComponent<UITooltip>();
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid setting type");
                }

                tooltip.title = setting.NameInMenu;
                tooltip.description = setting.TooltipDescription;
            }

            Debug.Log("Settings > Instantiated Settings!");
        }

        private static string[] CollectCategories()
        {
            List<string> categories = new List<string>();

            foreach (ISetting setting in Settings.GetAll())
            {
                if (!categories.Contains(setting.Category.name))
                    categories.Add(setting.Category.name);
            }

            return categories.ToArray();
        }

        private static ISetting[] GetSettingsInCategory(string category) => Settings.GetAll().AsValueEnumerable().Where(s => s.Category.name == category).ToArray();
    }
}
