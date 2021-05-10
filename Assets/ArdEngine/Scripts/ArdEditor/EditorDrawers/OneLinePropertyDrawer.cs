using ArdEditor.EditorTools;
using ArdEngine.ArdAttributes;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.EditorDrawers
{
    [CustomPropertyDrawer(typeof(OneLinePropertyAttribute))]
    public class OneLinePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            int propertyCount = property.Copy().CountInProperty() - 1;
            float fieldWidth = position.width / propertyCount;
            Rect rect = position;
            rect.width = fieldWidth;

            for (var i = 0; i < propertyCount; i++)
            {
                property.NextVisible(true);
                EditorGUI.PropertyField(rect, property, GUIContent.none, false);
                rect.x += fieldWidth;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetMaxPropertyHeight();
        }
    }
}