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
#if BG_ENABLE_LOCALIZATION
using UnityEngine.Localization.Components;
#endif
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
        [SerializeField] private HorizontalMultiChoiceOptionUIMetadata horizontalMultiChoicePrefab;
        [SerializeField] private ButtonOptionUIMetadata buttonPrefab;
        [SerializeField] private KeybindOptionUIMetadata keybindPrefab;
        [SerializeField] private OptionUIMetadata dividerPrefab;

        [Header("Keybinds")]
        [SerializeField] private PublicUIBehaviour rebindingScreen;

        private readonly HashSet<string> _settingsModified = new();

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
                    Debug.Log($"Settings > Apply: {settingObject.ID}");
                    settingObject.OnApply();
                }

                _settingsModified.Clear();

                Hide();
            });
        }

        private void Start()
        {
            // Collect Categories
            List<CategoryDefinition> categories = new List<CategoryDefinition>();
            categories.AddRange(CollectCategories());
            categories = categories
#if ZLINQ
                .AsValueEnumerable()
#endif
                .OrderBy(category => category.OrderIndex)
                .ToList();

            foreach (CategoryDefinition category in categories)
            {
                Button categoryButton = Instantiate(categoryPrefab, categoryParent);
#if BG_ENABLE_LOCALIZATION
                if (category.LocalizeDisplayName)
                {
                    LocalizeStringEvent stringEvent = categoryButton.GetComponentInChildren<LocalizeStringEvent>();
                    stringEvent.StringReference = category.LocalizedDisplayName;
                    stringEvent.RefreshString();
                }
                else
#endif
                {
                    categoryButton.GetComponentInChildren<TMP_Text>().text = category.name;
                }
                categoryButton.onClick.AddListener(() => InstantiateSettingsInCategory(category));
            }

            InstantiateSettingsInCategory(categories[0]);
        }

        private void InstantiateSettingsInCategory(CategoryDefinition category)
        {
            settingsParent.ClearAllChildren();

            List<ISetting> settings = new List<ISetting>();
            settings.AddRange(GetSettingsInCategory(category));
            settings.Sort((setting1, setting2) => setting1.OrderIndex.CompareTo(setting2.OrderIndex));

            foreach (ISetting setting in settings)
            {
                OptionUIMetadata option = CreateOptionUI(setting);
                UITooltip tooltip = option.GetComponent<UITooltip>();

                if (!setting.Description.IsEmpty)
                    tooltip.SetTooltip(setting.NameInMenu, setting.Description);
                else
                    tooltip.SetTooltip(string.Empty, string.Empty);
            }
        }

        private OptionUIMetadata CreateOptionUI(ISetting setting)
        {
            OptionUIMetadata option = setting switch
            {
                ButtonSetting buttonSetting => CreateButtonOptionUI(buttonSetting),
                EnumSetting enumSetting => CreateEnumOptionUI(enumSetting),
                FloatSetting floatSetting => CreateFloatOptionUI(floatSetting),
                IntSetting intSetting => CreateIntOptionUI(intSetting),
                KeybindSetting keybindSetting => CreateKeybindOptionUI(keybindSetting),
                BoolSetting boolSetting => CreateBoolOptionUI(boolSetting),
                SettingDivider settingDivider => CreateSettingDividerOptionUI(settingDivider),
                _ => throw new ArgumentOutOfRangeException(nameof(setting))
            };
            
#if BG_ENABLE_LOCALIZATION
            option.Title.StringReference = setting.NameInMenu;
            option.Title.RefreshString();
#else
            option.Title.text = setting.NameInMenu;
#endif
            option.Dirty += () => _settingsModified.Add(setting.ID);
            
            return option;
        }

        private OptionUIMetadata CreateButtonOptionUI(ButtonSetting buttonSetting)
        {
            ButtonOptionUIMetadata buttonOption = Instantiate(buttonPrefab, settingsParent);
            buttonOption.Button.onClick.AddListener(buttonSetting.TriggerButtonPress);
            
            return buttonOption;
        }

        private OptionUIMetadata CreateEnumOptionUI(EnumSetting enumSetting)
        {
            return enumSetting.Style switch
            {
                EnumSetting.DisplayStyle.Dropdown => CreateDropdownOptionUI(enumSetting, AddOptions),
                EnumSetting.DisplayStyle.Horizontal => CreateHorizontalMultiChoiceOptionUI(enumSetting, AddOptions),
                _ => throw new ArgumentOutOfRangeException()
            };

            void AddOptions(EnumOptionUIMetadata enumOptionUI)
            {
#if BG_ENABLE_LOCALIZATION
                if (enumSetting.LocalizedOptions)
                {
                    enumOptionUI.SetItems(enumSetting.Options
#if ZLINQ
                        .AsValueEnumerable()
#endif
                        .Select(option => option.localizedDisplayName)
                        .ToArray());
                }
                else
#endif
                {
                    enumOptionUI.SetItems(enumSetting.Options
#if ZLINQ
                        .AsValueEnumerable()
#endif
                        .Select(option => option.displayNameString)
                        .ToArray());
                }
            }
        }

        private EnumOptionUIMetadata CreateDropdownOptionUI(EnumSetting enumSetting, Action<EnumOptionUIMetadata> addOptions)
        {
            DropdownOptionUIMetadata dropdownOption = Instantiate(dropdownPrefab, settingsParent);
            addOptions(dropdownOption);
            dropdownOption.Dropdown.value = enumSetting.InternalIndex;
            dropdownOption.Dirty += () => enumSetting.SetValueByIndex(dropdownOption.Dropdown.value);

            return dropdownOption;
        }

        private EnumOptionUIMetadata CreateHorizontalMultiChoiceOptionUI(EnumSetting enumSetting, Action<EnumOptionUIMetadata> addOptions)
        {
            HorizontalMultiChoiceOptionUIMetadata horizontalMultiChoiceOption = Instantiate(horizontalMultiChoicePrefab, settingsParent);
            addOptions(horizontalMultiChoiceOption);
            horizontalMultiChoiceOption.HorizontalMultiChoice.SetValue(enumSetting.InternalIndex);
            horizontalMultiChoiceOption.Dirty += () => enumSetting.SetValueByIndex(horizontalMultiChoiceOption.HorizontalMultiChoice.Value);

            return horizontalMultiChoiceOption;
        }

        private OptionUIMetadata CreateFloatOptionUI(FloatSetting floatSetting)
        {
            FloatOptionUIMetadata floatOption = Instantiate(floatPrefab, settingsParent);
            floatOption.Slider.maxValue = floatSetting.MaxValue;
            floatOption.Slider.minValue = floatSetting.MinValue;
            floatOption.Slider.value = floatSetting.InternalValue;
            floatOption.Dirty += () => floatSetting.Value = floatOption.Slider.value;

            return floatOption;
        }

        private OptionUIMetadata CreateIntOptionUI(IntSetting intSetting)
        {
            IntOptionUIMetadata intOption = Instantiate(intPrefab, settingsParent);
            intOption.Slider.maxValue = intSetting.MaxValue;
            intOption.Slider.minValue = intSetting.MinValue;
            intOption.Slider.value = intSetting.InternalValue;
            intOption.Dirty += () => intSetting.Value = Mathf.RoundToInt(intOption.Slider.value);

            return intOption;
        }

        private OptionUIMetadata CreateKeybindOptionUI(KeybindSetting keybindSetting)
        {
            KeybindOptionUIMetadata keybindOption = Instantiate(keybindPrefab, settingsParent);

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

            return keybindOption;
            
            void UpdateText()
            {
                keybindOption.ButtonText.text = keybindSetting.InputAction.GetBindingDisplayString(keybindSetting.BindingIndex);
            }
        }

        private OptionUIMetadata CreateBoolOptionUI(BoolSetting boolSetting)
        {
            CheckboxOptionUIMetadata checkboxOption = Instantiate(checkboxPrefab, settingsParent);
            checkboxOption.Checkbox.isOn = boolSetting.InternalValue;
            checkboxOption.Dirty += () => boolSetting.Value = checkboxOption.Checkbox.isOn;
            
            return checkboxOption;
        }

        private OptionUIMetadata CreateSettingDividerOptionUI(SettingDivider _)
        {
            OptionUIMetadata dividerOption = Instantiate(dividerPrefab, settingsParent);
            return dividerOption;
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

        protected override void OnHiding()
        {
            foreach (ISetting settingObject in _settingsModified
#if ZLINQ
                         .AsValueEnumerable()
#endif
                         .Select(Settings.Get))
            {
                settingObject.DiscardValue();
            }
        }
    }
}
