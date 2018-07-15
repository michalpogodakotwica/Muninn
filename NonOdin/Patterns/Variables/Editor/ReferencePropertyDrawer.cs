using UnityEditor;
using UnityEngine;

namespace Patterns.Variables.Editor
{
    public class ReferencePropertyDrawer : PropertyDrawer
    {
        private const float PopupWidth = 65f;
        private const float Spacing = 4f;
        private readonly string[] _popupOptions = {"Variable", "Constant"};

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.PropertyField(new Rect(rect.position.x, rect.y, rect.width - PopupWidth - Spacing, rect.height),
                property.FindPropertyRelative("UseConstant").boolValue
                    ? property.FindPropertyRelative("ConstantValue")
                    : property.FindPropertyRelative("Variable"), new GUIContent(property.displayName));

            var value = property.FindPropertyRelative("UseConstant").boolValue ? 1 : 0;
            value = EditorGUI.Popup(
                new Rect(rect.position.x + rect.width - PopupWidth, rect.y, PopupWidth, rect.height),
                value, _popupOptions);
            property.FindPropertyRelative("UseConstant").boolValue = value == 1;
            if (value == 1)
                property.FindPropertyRelative("Variable").objectReferenceValue = null;
            EditorGUI.EndProperty();
        }
    }
}