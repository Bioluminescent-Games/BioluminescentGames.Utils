using System.Reflection;
using BioluminescentGames.Utils.Runtime;
using BioluminescentGames.Utils.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BioluminescentGames.Utils.Editor.Utilities
{
    [CustomPropertyDrawer(typeof(BindingIndexAttribute))]
    public class BindingIndexDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BindingIndexAttribute attr = (BindingIndexAttribute)attribute;

            MemberInfo actionReferenceField = ReflectionUtils.GetValidMemberInfo(attr.ActionReferenceField, property);

            var actionRef = ReflectionUtils.GetMemberInfoValue(actionReferenceField, property) as InputActionReference;
            if (actionRef == null)
            {
                EditorGUI.HelpBox(position, "Assign an Action Reference first", MessageType.Info);
                return;
            }

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

            options = options
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Select(s => s.Replace("/", "_"))
                .ToArray(); // Remove / to avoid making submenus

            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, options);
        }
    }
}
