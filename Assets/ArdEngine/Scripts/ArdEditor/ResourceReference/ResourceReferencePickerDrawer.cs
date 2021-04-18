using System;
using System.Reflection;
using ArdEditor.EditorSettings;
using ArdEditor.EditorTools;
using ArdEngine.DataTools;
using ArdEngine.ResourceReference;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    [CustomPropertyDrawer(typeof(ResourceReferenceAttribute), true)]
    public sealed class ResourceReferencePickerDrawer : PropertyDrawer
    {
        private bool _initialized;
        private ArdEngineSettings _settings;
        private string[] _keys;
        private string _keyString;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "SVRPicker requires int field!", MessageType.Error);
                return;
            }
            
            if (_initialized == false)
            {
                Initialize(property);
            }

            EditorGUI.LabelField(position, property.displayName, _keyString, EditorStyles.popup);
            if (Event.current.type == EventType.MouseUp && position.Contains(Event.current.mousePosition))
            {
                AutocompleteWindow.ShowBelow(position, _keys, index =>
                {
                    _keyString = _keys[index];
                    property.intValue = _keyString.GetStableHash();
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }

        private void Initialize(SerializedProperty property)
        {
            _settings = ArdEngineSettings.GetOrCreateSettings();
            var attributeData = (ResourceReferenceAttribute) attribute;
            var assetPath = $"{_settings.DataPath}/{attributeData.DataType.Name}.asset";
            var set = AssetDatabase.LoadAssetAtPath<ResourceReferenceSet>(assetPath);
            _keys = new string[0];
            if (set != null)
            {
                _keys = ExtractKeys(attributeData, set);
            }
            
            int keyHash = property.intValue;
            _keyString = $"<<{keyHash}>>";
            for (var i = 0; i < _keys.Length; i++)
            {
                if (keyHash == _keys[i].GetStableHash())
                {
                    _keyString = _keys[i];
                    break;
                }
            }

            _initialized = true;
        }

        private static string[] ExtractKeys(ResourceReferenceAttribute attributeData, ResourceReferenceSet set)
        {
            FieldInfo setData = attributeData.DataType.GetField("_data",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (setData != null)
            {
                var data = (Array) setData.GetValue(set);
                Type elementType = data.GetValue(0).GetType();
                FieldInfo keyField = elementType.GetField(ResourceReferenceGenerator.KEY_FIELD,
                    BindingFlags.Instance | BindingFlags.Public);
                var keys = new string[data.Length];
                var index = 0;

                foreach (object element in data)
                {
                    keys[index++] = (string) keyField?.GetValue(element);
                }

                return keys;
            }

            return new string[0];
        }
    }
}