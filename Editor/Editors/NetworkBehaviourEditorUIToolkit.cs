using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using Unity.Netcode.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace BioluminescentGames.Utils.Editor.Editors
{
    // Source: https://discussions.unity.com/t/feature-request-ui-toolkit-editor-for-networkbehaviour/1704793

    [CustomEditor(typeof(NetworkBehaviour), true)]
    [CanEditMultipleObjects]
    public class NetworkBehaviourEditorUIToolkit : UnityEditor.Editor
    {
        private bool m_Initialized;
        private readonly List<string> m_NetworkVariableNames = new();
        private readonly Dictionary<string, FieldInfo> m_NetworkVariableFields = new();

        private GUIContent m_NetworkVariableLabelGuiContent;
        private GUIContent m_NetworkListLabelGuiContent;

        internal const string CheckForNetworkObjectKey = "NetworkBehaviour-Check-For-NetworkObject";

        private void OnEnable()
        {
            if (target == null)
                return;

            NetworkBehaviourEditor.CheckForNetworkObject((target as NetworkBehaviour).gameObject);
        }

        private void Init(MonoScript script)
        {
            m_Initialized = true;
            m_NetworkVariableNames.Clear();
            m_NetworkVariableFields.Clear();

            m_NetworkVariableLabelGuiContent = new GUIContent("NetworkVariable", "This variable is a NetworkVariable. It can not be serialized and can only be changed during runtime.");
            m_NetworkListLabelGuiContent = new GUIContent("NetworkList", "This variable is a NetworkList. It is rendered, but you can't serialize or change it.");

            var fields = script.GetClass().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var f in fields)
            {
                if (f.IsDefined(typeof(HideInInspector), true) ||
                    f.IsDefined(typeof(NonSerializedAttribute), true))
                    continue;

                if (f.FieldType.IsGenericType &&
                    (f.FieldType.GetGenericTypeDefinition() == typeof(NetworkVariable<>) ||
                     f.FieldType.GetGenericTypeDefinition() == typeof(NetworkList<>)))
                {
                    var niceName = ObjectNames.NicifyVariableName(f.Name);
                    m_NetworkVariableNames.Add(niceName);
                    m_NetworkVariableFields[niceName] = f;
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (!m_Initialized)
            {
                var script = serializedObject.FindProperty("m_Script").objectReferenceValue as MonoScript;
                if (script != null)
                    Init(script);
            }

            var root = new VisualElement();

            foreach (var name in m_NetworkVariableNames)
            {
                if (!m_NetworkVariableFields.TryGetValue(name, out var field))
                    continue;

                object value = field.GetValue(target);
                if (value == null)
                {
                    value = Activator.CreateInstance(field.FieldType, true);
                    field.SetValue(target, value);
                }

                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(NetworkVariable<>))
                {
                    AddNetworkVariableField(root, name, value, field.FieldType.GetGenericArguments()[0]);
                }
                else if (field.FieldType.IsGenericType &&
                         field.FieldType.GetGenericTypeDefinition() == typeof(NetworkList<>))
                {
                    AddNetworkListField(root, name, value);
                }
            }

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            return root;
        }

        private void AddNetworkVariableField(VisualElement root, string name, object networkVariableObj, Type type)
        {
            var networkVariableBase = networkVariableObj as dynamic; // NetworkVariable<T>
            VisualElement fieldElement = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };

            if (type.IsValueType || type.IsEnum || type == typeof(string) || type == typeof(bool))
            {
                if (target is NetworkBehaviour behaviour && IsBehaviourEditable(behaviour))
                {
                    VisualElement editorField = type switch
                    {
                        var t when t == typeof(int) => new IntegerField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(uint) => new LongField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(short) => new IntegerField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(ushort) => new IntegerField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(sbyte) => new IntegerField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(byte) => new IntegerField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(long) => new LongField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(ulong) => new LongField(name) { value = (long)(ulong)networkVariableBase.Value },
                        var t when t == typeof(float) => new FloatField(name) { value = networkVariableBase.Value },
                        var t when t == typeof(bool) => new Toggle(name) { value = networkVariableBase.Value },
                        var t when t == typeof(string) => new TextField(name) { value = networkVariableBase.Value },
                        var t when t.IsEnum => new EnumField(name, (Enum)networkVariableBase.Value),
                        _ => new Label(name + ": Type not renderable")
                    };

                    editorField.RegisterCallback<ChangeEvent<object>>(evt =>
                    {
                        networkVariableBase.Value = evt.newValue;
                    });

                    fieldElement.Add(editorField);
                }
                else
                {
                    fieldElement.Add(new Label(name + ": " + networkVariableBase.Value));
                }
            }
            else
            {
                fieldElement.Add(new Label(name + ": Type not renderable"));
            }

            // Add mini label for NetworkVariable
            fieldElement.Add(new Label(m_NetworkVariableLabelGuiContent.text) { tooltip = m_NetworkVariableLabelGuiContent.tooltip, style = { unityTextAlign = TextAnchor.MiddleLeft } });
            root.Add(fieldElement);
        }
        private bool IsBehaviourEditable(NetworkBehaviour behaviour)
        {
            if (behaviour == null) return false;

            var netObj = behaviour.NetworkObject;
            var netMgr = NetworkManager.Singleton;

            if (netObj == null || netMgr == null || !netMgr.IsListening)
                return true;

            // Only the authority can modify
            return netObj.IsOwner;
        }


        private void AddNetworkListField(VisualElement root, string name, object networkListObj)
        {
            VisualElement fieldElement = new() { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            string value = "";
            bool addComma = false;

            foreach (var v in (IEnumerable<dynamic>)networkListObj)
            {
                if (addComma) value += ", ";
                value += v.ToString();
                addComma = true;
            }

            fieldElement.Add(new Label(name + ": " + value));
            fieldElement.Add(new Label(m_NetworkListLabelGuiContent.text) { tooltip = m_NetworkListLabelGuiContent.tooltip, style = { unityTextAlign = TextAnchor.MiddleLeft } });
            root.Add(fieldElement);
        }
    }
}
