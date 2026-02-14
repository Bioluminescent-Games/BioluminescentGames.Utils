using BioluminescentGames.Utils.Utilities;
using UnityEditor;
using UnityEngine;

namespace BioluminescentGames.Utils.Editor
{
    [CustomEditor(typeof(ScriptableEvent))]
    public class ScriptableEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ScriptableEvent scriptableEvent = (ScriptableEvent)target;

            if (GUILayout.Button("Invoke Event"))
                scriptableEvent.Trigger();
        }
    }
}
