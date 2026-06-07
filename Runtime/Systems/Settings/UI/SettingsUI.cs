using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;
using BioluminescentGames.Utils.Systems.Settings.UI.Metadata;
using BioluminescentGames.Utils.Systems.UI;
using BioluminescentGames.Utils.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BioluminescentGames.Utils.Systems.Settings.UI
{
    public class SettingsUI : BioluminescentPublicUIBehaviour
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
        [SerializeField] private KeybindOptionUIMetadata keybindPrefab;
        [SerializeField] private OptionUIMetadata dividerPrefab;

        [Header("Keybinds")]
        [SerializeField] private PublicUIBehaviour rebindingScreen;

        private readonly HashSet<string> _settingsModified = new HashSet<string>();

        private void Awake()
        {
            applyButton.onClick.AddListener(() =>
            {
                foreach (ISetting settingObject in _settingsModified
#if ZLINQ
                             .AsValueEnumerable()
#endif
                             .Select(Settings.Get))
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
            List<CategoryDefinition> categories = new List<CategoryDefinition>();
            categories.AddRange(CollectCategories());
            categories = categories
#if ZLINQ
                             .AsValueEnumerable()
#endif
                .OrderBy(category => category)
                .ToList(); // Sort Alphabetically so it isn't random order.

            foreach (CategoryDefinition category in categories)
            {
                Button categoryButton = Instantiate(categoryPrefab, categoryParent);
                categoryButton.GetComponentInChildren<TMP_Text>().text = category.name;
                categoryButton.onClick.AddListener(() => InstantiateSettingsInCategory(category));
            }

            InstantiateSettingsInCategory(categories[0]);
        }

        private void InstantiateSettingsInCategory(CategoryDefinition category)
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
                    case KeybindSetting keybindSetting:
                        KeybindOptionUIMetadata keybindOption = Instantiate(keybindPrefab, settingsParent);
                        keybindOption.Title.text = keybindSetting.NameInMenu;

                        keybindOption.Button.onClick.AddListener(() =>
                        {
                            bool actionEnabled = keybindSetting.InputAction.enabled;
                            keybindSetting.InputAction.Disable();

                            rebindingScreen.ShowObject();
                            keybindSetting.InputAction.PerformInteractiveRebinding(keybindSetting.BindingIndex)
                                .WithCancelingThrough(Keyboard.current.escapeKey)
                                .WithControlsExcluding("Mouse")
                                .OnMatchWaitForAnother(0.1f)
                                .OnCancel(o =>
                                {
                                    o.Dispose();
                                    rebindingScreen.HideObject();
                                    if (actionEnabled)
                                        keybindSetting.InputAction.Enable();
                                })
                                .OnComplete(o =>
                                {
                                    o.Dispose();
                                    UpdateText();
                                    rebindingScreen.HideObject();
                                    keybindSetting.OnApply();
                                    if (actionEnabled)
                                        keybindSetting.InputAction.Enable();
                                })
                                .Start();
                        });

                        UpdateText();

                        keybindOption.ResetButton.onClick.AddListener(() =>
                        {
                            keybindSetting.InputAction.RemoveBindingOverride(keybindSetting.BindingIndex);
                            keybindSetting.OnApply();
                            UpdateText();
                        });

                        tooltip = keybindOption.GetComponent<UITooltip>();
                        break;

                        void UpdateText()
                        {
                            keybindOption.ButtonText.text = keybindSetting.InputAction.GetBindingDisplayString(keybindSetting.BindingIndex) + " [Rebind]";
                        }

                    case SettingDivider settingDivider:
                        OptionUIMetadata dividerOption = Instantiate(dividerPrefab, settingsParent);
                        dividerOption.Title.text = settingDivider.NameInMenu;

                        tooltip = dividerOption.GetComponent<UITooltip>();
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid setting type");
                }

                if (!string.IsNullOrWhiteSpace(setting.TooltipDescription))
                {
                    tooltip.title = setting.NameInMenu;
                    tooltip.description = setting.TooltipDescription;
                }
                else
                {
                    tooltip.title = string.Empty;
                    tooltip.description = string.Empty;
                }
            }

            Debug.Log("Settings > Instantiated Settings!");
        }

        private static CategoryDefinition[] CollectCategories()
        {
            List<CategoryDefinition> categories = new();

            foreach (ISetting setting in Settings.GetAll())
            {
                if (!categories.Contains(setting.Category))
                    categories.Add(setting.Category);
            }

            categories.Sort();
            return categories.ToArray();
        }

        public override void OnUpdate()
        {
            applyButton.gameObject.SetActive(_settingsModified.Count > 0);
        }

        private static ISetting[] GetSettingsInCategory(CategoryDefinition category) => Settings.GetAll()
#if ZLINQ
                             .AsValueEnumerable()
#endif
            .Where(s => s.Category == category)
            .ToArray();
    }
}
