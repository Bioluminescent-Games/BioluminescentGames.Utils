using System;
using System.Diagnostics;
using System.IO;
using BioluminescentGames.Utils.StaticUtilities;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;
#if ALCHEMY_INSPECTOR
using Alchemy.Inspector;
#endif
using Void = BioluminescentGames.Utils.Runtime.StaticUtilities.Void;

namespace BioluminescentGames.Utils.Components
{
    public class OpenExplorerComponent : MonoBehaviour
    {
        private enum Path
        {
            InstallLocation,
            DataPath,
            PersistentDataPath,
            StreamingAssets,
            CustomWithPlaceholders,
        }
        private enum Type
        {
            File,
            Folder
        }

#if ALCHEMY_INSPECTOR
        [HelpBox("Supplies 2 UnityEvent compatible functions. You can also use ScriptableEvents")]
        [SerializeField] private Void helpBoxVoid;
#endif
        [SerializeField] private ScriptableEvent scriptableEvent;
        [SerializeField] private ScriptableEvent<string> scriptableEventWithPath;

        [Space(2)]
        [SerializeField] private Type type;
        [SerializeField] private Path pathPreset;

        [Space(2)]
        
#if ALCHEMY_INSPECTOR
        
        [ShowIf(nameof(ShowPlaceholderStuff))]
        [HelpBox("Valid Placeholders:\n[InstallLocation] = Install Location\n[DP] = DataPath\n[PDP] = PersistentDataPath\n[SA] = StreamingAssets")]
        [SerializeField] private Void placeholdersVoid;
        
        private bool ShowPlaceholderStuff => pathPreset == Path.CustomWithPlaceholders;
        
        [ShowIf(nameof(ShowPlaceholderStuff))]
#endif
        [SerializeField] private string pathToOpen = "";

        private string InstallDirectory => Directory.GetParent(Application.dataPath)!.FullName;

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
                    OpenExplorerToPath(InstallDirectory);
                    break;
                case Path.DataPath:
                    OpenExplorerToPath(Application.dataPath);
                    break;
                case Path.PersistentDataPath:
                    OpenExplorerToPath(Application.persistentDataPath);
                    break;
                case Path.StreamingAssets:
                    OpenExplorerToPath(Application.streamingAssetsPath);
                    break;
                case Path.CustomWithPlaceholders:
                    string path = pathToOpen.Replace("[InstallLocation]", InstallDirectory).Replace("[DP]", Application.dataPath).Replace("[PDP]", Application.persistentDataPath)
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
                        Log.Error("File not found: " + path);
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
                        Log.Error("Directory not found: " + path);
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
