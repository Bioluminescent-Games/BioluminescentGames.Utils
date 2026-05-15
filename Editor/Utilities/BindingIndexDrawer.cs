using BioluminescentGames.Utils.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using ZLinq;

namespace BioluminescentGames.Utils.Editor.Utilities
{
    [CustomPropertyDrawer(typeof(BindingIndexAttribute))]
    public class BindingIndexDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BindingIndexAttribute attr = (BindingIndexAttribute)attribute;

            /*SerializedProperty actionRefProp = property.serializedObject
                .FindProperty(attr.ActionReferenceField);*/
            SerializedProperty actionRefProp = FindActionRefProperty(property, attr.ActionReferenceField);

            if (actionRefProp == null || actionRefProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(position, "Assign an Action Reference first", MessageType.Info);
                return;
            }

            InputActionReference actionRef = (InputActionReference)actionRefProp.objectReferenceValue;
            InputAction action = actionRef.action;
            ReadOnlyArray<InputBinding> bindings = action.bindings;

            // Build dropdown options
            string[] options = new string[bindings.Count];
            for (int i = 0; i < bindings.Count; i++)
            {
                InputBinding b = bindings[i];
                if (b.isComposite)
                    options[i] = $"{i}: [{b.GetNameOfComposite()}] (composite)";
                else if (b.isPartOfComposite)
                    options[i] = $"{i}: {b.name} - {b.effectivePath}";
                else
                    options[i] = $"{i}: {b.effectivePath} [{b.groups.TrimStart(';')}]";
            }

            options = options.AsValueEnumerable()
                .Select(s => s.Replace("/", "_"))
                .ToArray(); // Remove / to avoid making submenus

            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, options);
        }

        private static SerializedProperty FindActionRefProperty(SerializedProperty property, string fieldName)
        {
            SerializedObject so = property.serializedObject;

            // Plain field
            SerializedProperty found = so.FindProperty(fieldName);
            if (found != null) return found;

            // Backing field for auto-properties
            found = so.FindProperty($"<{fieldName}>k__BackingField");
            return found;
        }
    }
}
