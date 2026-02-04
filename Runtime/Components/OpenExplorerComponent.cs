#if EDITOR_ATTRIBUTES

using System;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;
using EditorAttributes;

namespace BioluminescentGames.Utils.Components
{
    public class OpenExplorerComponent : MonoBehaviour
    {
        [HelpBox("Supplies 2 UnityEvent compatible functions. You can also use ScriptableEvents")]
        [SerializeField] private ScriptableEvent scriptableEvent;
        [SerializeField] private ScriptableEvent<string> scriptableEventWithPath;

        private enum Path
        {
            InstallLocation,
            PersistentDataPath,
            CustomWithPlaceholders,
        }

        [SerializeField] private Path pathPreset;

        [ShowField(nameof(pathPreset), Path.CustomWithPlaceholders)]
        [HelpBox("Valid Placeholders:\n[InstallLocation] = Install Location\n[PDP] = PersistentDataPath")]
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
                case Path.CustomWithPlaceholders:
                    string path = pathToOpen.Replace("[InstallLocation]", Application.dataPath).Replace("[PDP]", Application.persistentDataPath);
                    OpenExplorerToPath(path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OpenExplorerToPath(string path)
        {
            Application.OpenURL($"file:///{path}");
        }
    }
}
#endif
