#if EDITOR_ATTRIBUTES

using System;
using System.Diagnostics;
using System.IO;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;
using EditorAttributes;
using Debug = UnityEngine.Debug;

namespace BioluminescentGames.Utils.Components
{
    public class OpenExplorerComponent : MonoBehaviour
    {
        [HelpBox("Supplies 2 UnityEvent compatible functions. You can also use ScriptableEvents")]
        [SerializeField] private ScriptableEvent scriptableEvent;
        [SerializeField] private ScriptableEvent<string> scriptableEventWithPath;

        private enum Type
        {
            File,
            Folder
        }

        [SerializeField] private Type type;

        private enum Path
        {
            InstallLocation,
            PersistentDataPath,
            StreamingAssets,
            CustomWithPlaceholders,
        }

        [SerializeField] private Path pathPreset;

        [ShowField(nameof(pathPreset), Path.CustomWithPlaceholders)]
        [HelpBox("Valid Placeholders:\n[InstallLocation] = Install Location\n[PDP] = PersistentDataPath\n[SA] = StreamingAssets")]
        [SerializeField] private string pathToOpen = "";

        private void Awake()
        {
            if (scriptableEvent != null)
                scriptableEvent.EventTriggered += OpenExplorer;

            if (scriptableEventWithPath != null)
                scriptableEventWithPath.EventTriggered += OpenExplorerToPath;
        }

        public void OpenExplorer()
        {
            switch (pathPreset)
            {
                case Path.InstallLocation:
                    OpenExplorerToPath(Application.dataPath);
                    break;
                case Path.PersistentDataPath:
                    OpenExplorerToPath(Application.persistentDataPath);
                    break;
                case Path.StreamingAssets:
                    OpenExplorerToPath(Application.streamingAssetsPath);
                    break;
                case Path.CustomWithPlaceholders:
                    string path = pathToOpen.Replace("[InstallLocation]", Application.dataPath).Replace("[PDP]", Application.persistentDataPath)
                        .Replace("[SA]", Application.streamingAssetsPath);
                    OpenExplorerToPath(path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OpenExplorerToPath(string path)
        {
            switch (type)
            {
                case Type.File:
                    if (!File.Exists(path))
                    {
                        Debug.LogError("File not found: " + path);
                        return;
                    }

                    ProcessStartInfo startInfo = new()
                    {
                        FileName = System.IO.Path.GetFullPath(path),
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    break;
                case Type.Folder:
                    if (!Directory.Exists(path))
                    {
                        Debug.LogError("Directory not found: " + path);
                        return;
                    }

                    Application.OpenURL($"file:///{path}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif
