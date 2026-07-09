#region

using System.Collections.Generic;
using BioluminescentGames.Utils.Editor.Utilities;
using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

#endregion

namespace BioluminescentGames.Utils.Editor.Systems.Settings
{
    public class SettingsEditor : EditorWindow
    {
        private const string k_ToolName = "Settings Editor";
        private const string k_CategoryFolder = "Assets/Scriptable Objects/Settings Categories";
        private const string k_SettingsParentFolder = "Assets/Resources/Settings";

        private VisualElement _mainContainer;
        private ListView _leftPane;
        private ListView _rightPane;

        private List<Setting> _settingsToShow;

        [SerializeField, HideInInspector] private int categoriesSelectedIndex = -1;
        [SerializeField, HideInInspector] private int settingsSelectedIndex = -1;

        private Toolbar _toolbar;
        
        private ToolbarButton _removeCategoryToolbarButton;
        private Button _removeSettingButton;

        private List<CategoryDefinition> _categories;

        [MenuItem("Tools/Bioluminescent Games/" + k_ToolName)]
        private static void CreateWindow()
        {
            SettingsEditor window = GetWindow<SettingsEditor>();
            window.titleContent = new GUIContent(k_ToolName);
        }

        public void CreateGUI()
        {
            _toolbar = new Toolbar();
            _toolbar.Add(new ToolbarButton(BuildUI) { text = "Refresh" });
            _toolbar.Add(new ToolbarButton(AddCategory) { text = "Add Category" });

            _removeCategoryToolbarButton = new ToolbarButton(RemoveCategory) {text = "Remove Category"};

            rootVisualElement.Add(_toolbar);

            _mainContainer = new VisualElement { style = { flexGrow = 1 } };
            rootVisualElement.Add(_mainContainer);

            _removeSettingButton = new Button
            {
                text = "Remove Setting",
                style =
                {
                    width = StyleKeyword.Auto,
                    alignSelf = Align.FlexEnd,
                    position = Position.Absolute,
                    marginTop = 20
                }
            };

            _removeSettingButton.clicked += RemoveSetting;

            BuildUI();
        }

        private void AddCategory()
        {
            CategoryDefinition categoryDefinition = CreateInstance<CategoryDefinition>();
            AssetDatabaseUtils.CreateFolderTree(k_CategoryFolder);
            AssetDatabase.CreateAsset(categoryDefinition, $"{k_CategoryFolder}/New Category.asset");
            AssetDatabase.Refresh();
            BuildUI();
            UpdateSelectedCategory(categoryDefinition);
        }

        private void RemoveCategory()
        {
            if (categoriesSelectedIndex < 0 || categoriesSelectedIndex >= _categories.Count) return;

            CategoryDefinition category = _categories[categoriesSelectedIndex];

            if (!EditorUtility.DisplayDialog("Are you sure?",
                    $"Are you sure you'd like to remove the category {category.name}? WARNING: This cannot be undone!",
                    "Yes",
                    "No")) return;

            bool deleteRelatedFolderAndSettings = EditorUtility.DisplayDialog("Remove related files?",
                    $"Would you like to delete the settings in the category {category.name}? WARNING: This cannot be undone!",
                    "Yes",
                    "No");

            if (deleteRelatedFolderAndSettings)
            {
                string settingsFolder = k_SettingsParentFolder + "/" + category.name;
                if (AssetDatabase.IsValidFolder(settingsFolder))
                    AssetDatabase.DeleteAsset(settingsFolder);
            }

            string path = AssetDatabase.GetAssetPath(category);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();

            BuildUI();
            UpdateSelectedCategory(_categories[categoriesSelectedIndex]);
        }

        private void RemoveSetting()
        {
            if (_settingsToShow == null || settingsSelectedIndex < 0 || settingsSelectedIndex >= _settingsToShow.Count) return;

            Setting setting = _settingsToShow[settingsSelectedIndex];

            if (!EditorUtility.DisplayDialog("Are you sure?",
                    $"Are you sure you'd like to remove the setting {setting.name}? WARNING: This will permanently remove it, also removing all references. This cannot be undone.",
                    "Yes",
                    "No")) return;

            string path = AssetDatabase.GetAssetPath(setting);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();

            BuildUI();
            UpdateSelectedCategory(_categories[categoriesSelectedIndex]);
        }

        private void AddSetting<T>([CanBeNull] string nameOverride) where T : Setting
        {
            if (categoriesSelectedIndex < 0 || categoriesSelectedIndex >= _categories.Count) return;

            string folder = $"{k_SettingsParentFolder}/{_categories[categoriesSelectedIndex].name}";
            T setting = CreateInstance<T>();

            setting.EDITOR_SetCategory(_categories[categoriesSelectedIndex]);

            AssetDatabaseUtils.CreateFolderTree(folder);
            AssetDatabase.CreateAsset(setting, $"{folder}/New {nameOverride ?? typeof(T).Name}.asset");
            AssetDatabase.Refresh();

            UpdateSelectedCategory(_categories[categoriesSelectedIndex]);
        }

        private void AddSettingToMenu<T>(GenericDropdownMenu menu, [CanBeNull] string nameOverride = null) where T : Setting
        {
            if (categoriesSelectedIndex < 0 || categoriesSelectedIndex >= _categories.Count) return;
            menu.AddItem(nameOverride ?? typeof(T).Name, false, () => AddSetting<T>(nameOverride));
        }

        private void BuildUI()
        {
            _mainContainer.Clear();

            string[] categoryGuids = AssetDatabase.FindAssets("t:" + nameof(CategoryDefinition));
            _categories = categoryGuids
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(guid => AssetDatabase.LoadAssetAtPath<CategoryDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(category => category.OrderIndex)
                .ToList();

            for (int i = 0; i < _categories.Count; i++)
                _categories[i].EDITOR_SetOrderIndex(i);

            TwoPaneSplitView splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            _mainContainer.Add(splitView);

            _leftPane = new ListView
            {
                makeItem = () => {
                    TextField field = new() { isDelayed = true };

                    field.labelElement.style.display = DisplayStyle.None;
                    field.style.flexGrow = 1;

                    VisualElement inputNode = field.Q("unity-text-input");
                    inputNode.style.backgroundColor = new StyleColor(Color.clear);
                    inputNode.style.borderBottomWidth = 0;
                    inputNode.style.borderLeftWidth = 0;
                    inputNode.style.borderRightWidth = 0;
                    inputNode.style.borderTopWidth = 0;

                    return field;
                },
                bindItem = (item, index) => {
                    TextField textField = item as TextField;
                    CategoryDefinition category = _categories[index];

                    textField!.SetValueWithoutNotify(category.name);
                    textField.UnregisterCallback<ChangeEvent<string>>(OnCategoryRenamed);
                    textField.userData = category;
                    textField.RegisterValueChangedCallback(OnCategoryRenamed);

                    textField.RegisterCallback<FocusInEvent>(_ => {
                        textField.Q("unity-text-input").style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
                    });
                    textField.RegisterCallback<FocusOutEvent>(_ => {
                        textField.Q("unity-text-input").style.backgroundColor = new StyleColor(Color.clear);
                    });
                },
                itemsSource = _categories,
                selectionType = SelectionType.Single,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                selectedIndex = categoriesSelectedIndex
            };
            _leftPane.itemIndexChanged += LeftPaneOnItemIndexChanged;
            _leftPane.selectionChanged += LeftPaneOnSelectionChanged;
            _leftPane.selectionChanged += _ => categoriesSelectedIndex = _leftPane.selectedIndex;
            _leftPane.selectionChanged += _ => BuildUI();
            _leftPane.itemsChosen += OnItemsChosen;
            
            _leftPane.RegisterCallback<PointerDownEvent>(_ =>
            {
                if (categoriesSelectedIndex >= 0 && categoriesSelectedIndex < _categories.Count)
                    Selection.activeObject = _categories[categoriesSelectedIndex];
            });
            
            splitView.Add(_leftPane);
            
            _rightPane = new ListView
            {
                makeItem = () => {
                    TextField field = new() { isDelayed = true };

                    field.labelElement.style.display = DisplayStyle.None;
                    field.style.flexGrow = 1;

                    VisualElement inputNode = field.Q("unity-text-input");
                    inputNode.style.backgroundColor = new StyleColor(Color.clear);
                    inputNode.style.borderBottomWidth = 0;
                    inputNode.style.borderLeftWidth = 0;
                    inputNode.style.borderRightWidth = 0;
                    inputNode.style.borderTopWidth = 0;

                    return field;
                },
                bindItem = (item, index) =>
                {
                    if (_settingsToShow == null || index >= _settingsToShow.Count) return;

                    Setting setting = _settingsToShow[index];
                    TextField textField = item as TextField;

                    textField!.SetValueWithoutNotify(setting.name);
                    textField.UnregisterCallback<ChangeEvent<string>>(OnSettingRenamed);
                    textField.userData = setting;
                    textField.RegisterValueChangedCallback(OnSettingRenamed);

                    textField.RegisterCallback<FocusInEvent>(_ => {
                        textField.Q("unity-text-input").style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
                    });
                    textField.RegisterCallback<FocusOutEvent>(_ => {
                        textField.Q("unity-text-input").style.backgroundColor = new StyleColor(Color.clear);
                    });
                    //(item as Label)!.text = $"{setting.NameInMenu} <color=grey>({setting.ID})</color>";
                },
                itemsSource = _settingsToShow,
                selectionType = SelectionType.Single,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                selectedIndex = settingsSelectedIndex
            };
            _rightPane.selectionChanged += RightPaneOnSelectionChanged;
            _rightPane.itemIndexChanged += RightPaneOnItemIndexChanged;
            _rightPane.selectionChanged += _ => settingsSelectedIndex = _rightPane.selectedIndex;
            _rightPane.itemsChosen += OnItemsChosen;
            
            _rightPane.RegisterCallback<PointerDownEvent>(_ => 
            {
                if (_settingsToShow != null && settingsSelectedIndex >= 0 && settingsSelectedIndex < _settingsToShow.Count)
                    Selection.activeObject = _settingsToShow[settingsSelectedIndex];
            });
            
            splitView.Add(_rightPane);

            if (categoriesSelectedIndex >= 0 && categoriesSelectedIndex < _categories.Count)
            {
                _toolbar.Add(_removeCategoryToolbarButton);

                Button addButton = new()
                {
                    text = "Add Setting ▾",
                    style =
                    {
                        width = StyleKeyword.Auto,
                        alignSelf = Align.FlexEnd,
                        position = Position.Absolute,
                    }
                };

                addButton.clicked += () =>
                {
                    GenericDropdownMenu menu = new();
                    AddSettingToMenu<BoolSetting>(menu, "Toggle Setting");
                    AddSettingToMenu<ButtonSetting>(menu, "Button");
                    AddSettingToMenu<EnumSetting>(menu, "Dropdown Setting");
                    AddSettingToMenu<FloatSetting>(menu, "Decimal Number Setting");
                    AddSettingToMenu<IntSetting>(menu, "Whole Number Setting");
                    AddSettingToMenu<KeybindSetting>(menu, "Keybind Setting");
                    AddSettingToMenu<SettingDivider>(menu, "Divider");

                    menu.DropDown(addButton.worldBound, addButton, DropdownMenuSizeMode.Content);
                };

                _mainContainer.Add(addButton);

            } else if (_toolbar.Contains(_removeCategoryToolbarButton))
                _toolbar.Remove(_removeCategoryToolbarButton);

            if (_mainContainer.Contains(_removeSettingButton))
                _mainContainer.Remove(_removeSettingButton);
        }

        private void OnItemsChosen(IEnumerable<object> items)
        {
            foreach (object item in items)
            {
                if (item is Object unityObject)
                {
                    EditorGUIUtility.PingObject(unityObject);
                }
            }
        }

        private void RightPaneOnItemIndexChanged(int arg1, int arg2)
        {
            if (_settingsToShow == null) return;

            for (int i = 0; i < _settingsToShow.Count; i++)
            {
                // Assuming OrderIndex is public and settable on the Setting class
                _settingsToShow[i].EDITOR_SetOrderIndex(i);
                EditorUtility.SetDirty(_settingsToShow[i]);
            }
        }

        private void LeftPaneOnItemIndexChanged(int arg1, int arg2)
        {
            if (_leftPane.itemsSource is not List<CategoryDefinition> items) return;

            for (int i = 0; i < items.Count; i++)
            {
                items[i].EDITOR_SetOrderIndex(i);
                EditorUtility.SetDirty(items[i]);
            }
        }

        private void LeftPaneOnSelectionChanged(IEnumerable<object> selectedItems)
        {
            using IEnumerator<object> enumerator = selectedItems.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            CategoryDefinition selectedCategory = enumerator.Current as CategoryDefinition;

            UpdateSelectedCategory(selectedCategory);
            Selection.activeObject = selectedCategory;
        }

        private void UpdateSelectedCategory(CategoryDefinition selectedCategory)
        {
            if (!selectedCategory) return;
            string[] settingGuids = AssetDatabase.FindAssets("t:" + nameof(Setting));
            List<Setting> settingsToShow = settingGuids
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(guid => AssetDatabase.LoadAssetAtPath<Setting>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(s => s.Category == selectedCategory)
                .OrderBy(setting => setting.OrderIndex)
                .ToList();
            _settingsToShow = settingsToShow;
            for (int i = 0; i < _settingsToShow.Count; i++)
                _settingsToShow[i].EDITOR_SetOrderIndex(i);
            _rightPane.itemsSource = _settingsToShow;
            _rightPane.ClearSelection();
            _rightPane.Rebuild();
        }

        private void RightPaneOnSelectionChanged(IEnumerable<object> selectedItems)
        {
            if (_mainContainer.Contains(_removeSettingButton))
                _mainContainer.Remove(_removeSettingButton);

            using var enumerator = selectedItems.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            Setting selectedSetting = enumerator.Current as Setting;
            if (!selectedSetting) return;
            
            Selection.activeObject = selectedSetting;

            _mainContainer.Add(_removeSettingButton);
        }

        private static void OnCategoryRenamed(ChangeEvent<string> evt)
        {
            if (evt.target is TextField { userData: CategoryDefinition category })
                RenameAsset(category, evt.newValue);
        }

        private void OnSettingRenamed(ChangeEvent<string> evt)
        {
            if (evt.target is TextField { userData: Setting setting })
                RenameAsset(setting, evt.newValue);
        }

        private static void RenameAsset(Object asset, string newName)
        {
            if (string.IsNullOrEmpty(newName) || asset.name == newName) return;

            string path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
        }
    }
}
