using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.StaticUtilities;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif
using UnityEngine;
using Unity.Scripting.LifecycleManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BioluminescentGames.Utils.Editor
{
    [AutoStaticsCleanup]
    public static partial class FindMissingObjects
    {
        // https://gist.github.com/vertxxyz/0c3843b279ac821fe9f5b9b30c4a292c#file-findmissingobjects-cs
        
        [MenuItem("Tools/Missing Objects/Remove Missing Scripts from Selected GameObjects")]
        private static void RemoveMissingScriptsFromSelectedGameObjects()
        {
            int count = Selection.transforms
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Sum(transform => GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject));
            Log.Info($"Removed {count} missing scripts from selected GameObjects.");
        }
	
        [MenuItem("Tools/Missing Objects/Remove Missing Scripts from Selected GameObjects and Children")]
        private static void RemoveMissingScriptsFromSelectedGameObjectsAndChildren()
        {
            int count = 0;
            foreach (Transform root in Selection.transforms)
            {
                using (ListPool<Transform>.Get(out List<Transform> children))
                {
                    root.GetComponentsInChildren(true, children);
                    count += children
#if ZLINQ
                        .AsValueEnumerable()
#endif
                        .Sum(child => GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject));
                }
            }

            Log.Info($"Removed {count} missing scripts from selected GameObjects and their children.");
        }

        [MenuItem("Tools/Missing Objects/Log Missing Scripts in Project and Open Scenes")]
        private static void LogMissingScriptsInProject()
        {
            try
            {
                if (ProcessAllPrefabRootsInProject(LogMissingScriptsUnderRoot))
                    return;

                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Log.Warning("All scenes weren't searched because save dialog was cancelled.");
                    return;
                }

                if (ProcessRootGameObjectsInAllScenesAndRestore(LogMissingScriptsUnderRoot))
                    // ReSharper disable once RedundantJumpStatement
                    return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private static bool ProcessAllPrefabRootsInProject(Action<GameObject, string, Object> process)
	    {
		    string[] prefabPaths = AssetDatabase.GetAllAssetPaths()
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Where(path => path.EndsWith(".prefab")).ToArray();
		    int prefabCount = prefabPaths.Length;
		    float prefabCountLimit = Mathf.Max(1, prefabCount - 1);
		    for (var p = 0; p < prefabCount; p++)
		    {
			    float v = p / prefabCountLimit;

			    string path = prefabPaths[p];
			    if (EditorUtility.DisplayCancelableProgressBar("Finding missing scripts in prefabs", path, v))
				    return true;

			    process(AssetDatabase.LoadAssetAtPath<GameObject>(path), path, null);
		    }

		    return false;
	    }

	    private static bool ProcessRootGameObjectsInAllScenesAndRestore(Action<GameObject, string, Object> process)
	    {
		    SceneSetup[] sceneSetups = EditorSceneManager.GetSceneManagerSetup();
		    try
		    {
			    string[] scenePaths = AssetDatabase.GetAllAssetPaths()
#if ZLINQ
                    .AsValueEnumerable()
#endif
                    .Where(path => path.EndsWith(".unity")).ToArray();
			    int sceneCount = scenePaths.Length;
			    float sceneCountLimit = Mathf.Max(1, sceneCount - 1);
			    float sceneCountIncrement = 1 / sceneCountLimit;
			    for (var s = 0; s < sceneCount; s++)
			    {
                    try
                    {
                        float sceneV = s / sceneCountLimit;
                        string path = scenePaths[s];
                        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                        GameObject[] gameObjects = scene.GetRootGameObjects();
                        int gameObjectsCount = gameObjects.Length;
                        float gameObjectsCountLimit = Mathf.Max(1, gameObjectsCount - 1);
                        for (var g = 0; g < gameObjectsCount; g++)
                        {
                            float v = sceneV + g / gameObjectsCountLimit * sceneCountIncrement;
                            if (EditorUtility.DisplayCancelableProgressBar("Finding missing scripts in scenes", path,
                                    v))
                                return true;

                            process(gameObjects[g], path, AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
			    }
		    }
		    finally
		    {
			    EditorSceneManager.RestoreSceneManagerSetup(sceneSetups);
		    }

		    return false;
	    }

	    private static void LogMissingScriptsUnderRoot(GameObject gameObject, string path, Object pingOverride = null)
	    {
		    Transform root = gameObject.transform;
		    using (ListPool<Transform>.Get(out List<Transform> children))
		    {
			    gameObject.GetComponentsInChildren(true, children);
			    foreach (Transform child in children)
			    {
				    using (ListPool<Component>.Get(out List<Component> components))
				    {
					    child.GetComponents(components);
					    int missingScriptCount = components
#if ZLINQ
                            .AsValueEnumerable()
#endif
                            .Count(component => component == null);
					    if (missingScriptCount == 0) continue;
					    // ReSharper disable once Unity.NoNullCoalescing
					    Log.Warning($"\"{path} -> {AnimationUtility.CalculateTransformPath(child, root)} ({child.name})\" contains {missingScriptCount} missing scripts!",
						    pingOverride ?? gameObject);
				    }
			    }
		    }
	    }
    }
}
