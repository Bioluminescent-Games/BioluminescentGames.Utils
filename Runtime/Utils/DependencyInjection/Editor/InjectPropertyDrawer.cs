using UnityEditor;
using UnityEngine;

namespace BioluminescentGames.Utils.DependencyInjection
{
    [CustomPropertyDrawer(typeof(InjectAttribute))]
    public class InjectPropertyDrawer : PropertyDrawer
    {
        private Texture2D icon;

        private Texture2D LoadIcon()
        {
            if (icon == null)
            {
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Scripts/Utils/DependencyInjection/Editor/icon.png");
            }

            return icon;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            icon = LoadIcon();
            Rect iconRect = new Rect(position.x, position.y, 20.0f, 20.0f);
            position.xMin += 24.0f;

            if (icon != null)
            {
                Color savedColor = GUI.color;
                GUI.color = property.objectReferenceValue != null ? savedColor : Color.green;
                GUI.DrawTexture(iconRect, icon);
                GUI.color = savedColor;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}