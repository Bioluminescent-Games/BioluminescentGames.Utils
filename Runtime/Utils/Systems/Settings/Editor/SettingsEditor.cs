#region

using System.Collections.Generic;
using System.Linq;
using BioluminescentGames.Utils.Systems.Settings.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace BioluminescentGames.Utils.Systems.Settings.Editor
{
    public class SettingsEditor : EditorWindow
    {
        private const string k_ToolName = "Settings Editor";

        private VisualElement _mainContainer;
        private ListView _leftPane;
        private TwoPaneSplitView _rightPane;
        private ListView _rightPaneTopPane;
        private ScrollView _rightPaneBottomPane;

        private List<Setting> _settingsToShow;

        [SerializeField, HideInInspector] private int categoriesSelectedIndex = -1;
        [SerializeField, HideInInspector] private int settingsSelectedIndex = -1;

        [MenuItem("Tools/" + k_ToolName)]
        public static void ShowExample()
        {
            SettingsEditor window = GetWindow<SettingsEditor>();
            window.titleContent = new GUIContent(k_ToolName);
        }

        public void CreateGUI()
        {
            Toolbar toolbar = new();
            toolbar.Add(new ToolbarButton(BuildUI) { text = "Refresh" });
            rootVisualElement.Add(toolbar);

            _mainContainer = new VisualElement { style = { flexGrow = 1 } };
            rootVisualElement.Add(_mainContainer);

            BuildUI();
        }

        private void BuildUI()
        {
            _mainContainer.Clear();

            string[] categoryGuids = AssetDatabase.FindAssets("t:" + nameof(CategoryDefinition));
            List<CategoryDefinition> categories = categoryGuids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<CategoryDefinition>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            categories.Sort((a, b) => a.OrderIndex.CompareTo(b.OrderIndex));

            for (int i = 0; i < categories.Count; i++)
                categories[i].EDITOR_SetOrderIndex(i);

            TwoPaneSplitView splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            _mainContainer.Add(splitView);

            _leftPane = new ListView
            {
                makeItem = () => new Label(),
                bindItem = (item, index) => { (item as Label)!.text = categories[index].name; },
                itemsSource = categories,
                selectionType = SelectionType.Single,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                selectedIndex = categoriesSelectedIndex
            };
            _leftPane.itemIndexChanged += LeftPaneOnItemIndexChanged;
            _leftPane.selectionChanged += LeftPaneOnSelectionChanged;
            _leftPane.selectionChanged += _ => categoriesSelectedIndex = _leftPane.selectedIndex;
            _leftPane.itemsChosen += OnItemsChosen;
            splitView.Add(_leftPane);

            _rightPane = new TwoPaneSplitView(1, 250, TwoPaneSplitViewOrientation.Vertical);
            splitView.Add(_rightPane);

            _rightPaneTopPane = new ListView
            {
                makeItem = () => new Label(),
                bindItem = (item, index) =>
                {
                    if (_settingsToShow == null || index >= _settingsToShow.Count) return;

                    Setting setting = _settingsToShow[index];
                    (item as Label)!.text = $"{setting.NameInMenu} <color=grey>({setting.ID})</color>";
                },
                itemsSource = _settingsToShow,
                selectionType = SelectionType.Single,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                selectedIndex = settingsSelectedIndex
            };
            _rightPaneTopPane.selectionChanged += RightPaneTopPaneOnSelectionChanged;
            _rightPaneTopPane.itemIndexChanged += RightPaneTopPaneOnItemIndexChanged;
            _rightPaneTopPane.selectionChanged += _ => settingsSelectedIndex = _rightPaneTopPane.selectedIndex;
            _rightPaneTopPane.itemsChosen += OnItemsChosen;
            _rightPane.Add(_rightPaneTopPane);

            _rightPaneBottomPane = new ScrollView();
            _rightPane.Add(_rightPaneBottomPane);
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

        private void RightPaneTopPaneOnItemIndexChanged(int arg1, int arg2)
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
            if (!selectedCategory) return;

            string[] settingGuids = AssetDatabase.FindAssets("t:" + nameof(Setting));
            List<Setting> settings = settingGuids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<Setting>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            _settingsToShow = settings.Where(s => s.Category == selectedCategory).ToList();
            _settingsToShow.Sort((a, b) => a.OrderIndex.CompareTo(b.OrderIndex));
            for (int i = 0; i < _settingsToShow.Count; i++)
                _settingsToShow[i].EDITOR_SetOrderIndex(i);
            _rightPaneTopPane.itemsSource = _settingsToShow;
            _rightPaneTopPane.ClearSelection();
            _rightPaneTopPane.Rebuild();
        }

        private void RightPaneTopPaneOnSelectionChanged(IEnumerable<object> selectedItems)
        {
            _rightPaneBottomPane.Clear();

            using IEnumerator<object> enumerator = selectedItems.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            Setting selectedSetting = enumerator.Current as Setting;
            if (!selectedSetting) return;

            InspectorElement inspector = new(selectedSetting);
            _rightPaneBottomPane.Add(inspector);
        }
    }
}
