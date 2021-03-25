using ArdEngine.ResourceReference;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    [CustomPropertyDrawer(typeof(ResourceReferenceAttribute), true)]
    public sealed class ResourceReferencePickerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "SVRPicker requires int field!", MessageType.Error);
                return;
            }
            
            
        }
    }
}