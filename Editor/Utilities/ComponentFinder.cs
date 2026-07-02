using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.StaticUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

using Object = UnityEngine.Object;

public class ComponentFinder : EditorWindow
{
    private const string k_ToolName = "Component Finder";

    private ToolbarSearchField _searchField;
    private ToolbarButton _searchButton;
    private ToolbarToggle _searchInAssetsToggle;
    private ToolbarToggle _searchInSceneToggle;

    private ListView _componentList;

    [SerializeField] private VisualTreeAsset visualTreeAsset;

    private readonly List<ComponentProperties> _allComponents = new();
    private readonly List<ComponentProperties> _filteredComponents = new();

    private readonly struct ComponentProperties
    {
        public readonly Component Component;
        public readonly string AssetPath;
        public readonly string PathInAsset;
        public readonly string TypeName;
        public readonly bool IsPrefab;

        public ComponentProperties(Component component, string assetPath, bool isPrefab)
        {
            Component = component;
            AssetPath = assetPath;
            IsPrefab = isPrefab;

            PathInAsset = GeneratePath(component.transform);
            TypeName = component.GetType().GetName();
        }
    }

    [MenuItem("Tools/Bioluminescent Games/" + k_ToolName)]
    public static void CreateWindow()
    {
        ComponentFinder window = GetWindow<ComponentFinder>();
        window.titleContent = new GUIContent("Component Finder");
    }

    public void CreateGUI()
    {
        VisualElement visualTree = visualTreeAsset.Instantiate();
        rootVisualElement.Add(visualTree);

        _searchField = rootVisualElement.Q<ToolbarSearchField>("SearchField");
        _searchButton = rootVisualElement.Q<ToolbarButton>("SearchButton");
        _searchInAssetsToggle = rootVisualElement.Q<ToolbarToggle>("SearchInAssetsToggle");
        _searchInSceneToggle = rootVisualElement.Q<ToolbarToggle>("SearchInScenesToggle");

        _componentList = visualTree.Q<ListView>("ComponentList");
        _componentList.itemsSource = _filteredComponents;
        _componentList.makeItem = () => new Label();
        _componentList.bindItem = ComponentListBindItem;
        _componentList.selectionType = SelectionType.Single;
        _componentList.selectionChanged += ComponentListOnSelectionChanged;

        _searchButton.clicked += SearchButtonOnClicked;
    }

    private static void ComponentListOnSelectionChanged(IEnumerable<object> selectedItems)
    {
        using IEnumerator<object> enumerator = selectedItems.GetEnumerator();
        if (!enumerator.MoveNext()) return;

        if (enumerator.Current is not ComponentProperties selectedComponent)
            return;

        // sadly this cannot be done for prefabs :(
        if (!selectedComponent.IsPrefab && IsSceneOpen(selectedComponent.AssetPath))
        {
            EditorGUIUtility.PingObject(selectedComponent.Component);
            return;
        }

        Object containingThing = AssetDatabase.LoadAssetAtPath<Object>(selectedComponent.AssetPath);
        EditorGUIUtility.PingObject(containingThing);
    }

    private void ComponentListBindItem(VisualElement item, int index)
    {
        ComponentProperties componentPathPair = _filteredComponents[index];
        string assetPath = componentPathPair.AssetPath;
        string path = componentPathPair.PathInAsset;
        Label label = item as Label;
        Debug.Assert(label != null);
        label.text = $"{componentPathPair.TypeName} <color=grey>{assetPath}{path}</color>";
        //Object containingThing = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

    }

    private static string GeneratePath(Transform componentTransform, string currentPath = "")
    {
        Transform parent = componentTransform.parent;
        if (parent != null)
            currentPath += GeneratePath(parent, currentPath);

        return currentPath + "/" + componentTransform.gameObject.name;
    }

    private void SearchButtonOnClicked()
    {
        _allComponents.Clear();
        _filteredComponents.Clear();

        if (_searchInAssetsToggle.value)
            SearchInAssets();

        if (_searchInSceneToggle.value)
            SearchInScenes();

        FilterComponents();
        SortComponents();

        _componentList.RefreshItems();
        _componentList.selectedIndex = -1;
    }

    private void SortComponents()
    {
        _filteredComponents.Sort((a, b) => string.Compare(a.TypeName, b.TypeName, StringComparison.CurrentCulture));
    }

    private void FilterComponents()
    {
        string lowercaseSearchTerm = _searchField.value.ToLower();

        foreach (ComponentProperties component in _allComponents)
            if (component.TypeName.ToLower().Contains(lowercaseSearchTerm))
                _filteredComponents.Add(component);
    }

    private void SearchInAssets()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string prefabGuid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            if (IsPartOfPackage(path))
                continue;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            AddComponentsToList(prefab.GetComponentsInChildren<Component>(true), path, true);
        }
    }

    private void SearchInScenes()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        Scene[] oldOpenScenes = GetOpenScenes();

        foreach (string sceneGuid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuid);
            if (IsPartOfPackage(path))
                continue;

            if (SceneUtility.GetBuildIndexByScenePath(path) == -1)
                continue; // Ignore scenes not in build settings

            Scene openedScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            SearchInScene(openedScene);

            // Close scene if it wasn't open before
            if (oldOpenScenes
#if ZLINQ
                .AsValueEnumerable()
#endif
                .All(scene => scene.path != path))
                EditorSceneManager.CloseScene(openedScene, true);
        }
    }

    private void SearchInScene(Scene scene)
    {
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        foreach (GameObject rootGameObject in rootGameObjects)
            AddComponentsToList(rootGameObject.GetComponentsInChildren<Component>(true), scene.path, false);
    }

    private void AddComponentsToList(Component[] components, string path, bool isPrefab)
    {
        _allComponents.AddRange(components
#if ZLINQ
                .AsValueEnumerable()
#endif
            .Select(component => new ComponentProperties(component, path, isPrefab))
            .ToArray());
    }

    private static bool IsPartOfPackage(string path) => path.StartsWith("Packages/");

    private static Scene[] GetOpenScenes()
    {
        Scene[] scenes = new Scene[SceneManager.sceneCount];
        for (int i = 0; i < SceneManager.sceneCount; i++)
            scenes[i] = SceneManager.GetSceneAt(i);
        return scenes;
    }

    private static bool IsSceneOpen(string targetPath)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.path == targetPath && scene.isLoaded)
                return true;
        }
        return false;
    }
}
