using System.Collections.Generic;
using ArdEditor.EditorTools;
using ArdEngine.DataTools;
using ArdEngine.ResourceReference;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    [CustomEditor(typeof(ResourceReferenceSet), true)]
    public sealed class ResourceReferenceScriptableEditor : Editor
    {
        private SerializedProperty _data;
        private SearchableListUI _searchableList;
        private HashSet<int> _duplicateCheck;
        private HashSet<int> _duplicatedHash;

        private void OnEnable()
        {
            _data = serializedObject.FindProperty("_data");
            _duplicateCheck = new HashSet<int>();
            _duplicatedHash = new HashSet<int>();

            _searchableList = new SearchableListUI(
                new ReorderableList(serializedObject, _data, true, false, true, true)
                {
                    drawElementCallback = (rect, listIndex, active, focused) =>
                    {
                        SerializedProperty element = _data.GetArrayElementAtIndex(listIndex);
                        SerializedProperty key = element.FindPropertyRelative(ResourceReferenceGenerator.KEY_FIELD);
                        SerializedProperty value = element.FindPropertyRelative(ResourceReferenceGenerator.VALUE_FIELD);

                        Rect pos = rect;
                        pos.x += 14.0f;
                        pos.width -= 14.0f;
                        pos.height = EditorGUIUtility.singleLineHeight;
                        GUIStyle textStyle = EditorStyles.textField;
                        if (_duplicatedHash.Contains(key.stringValue.GetStableHash()))
                        {
                            textStyle = new GUIStyle(EditorStyles.textField)
                            {
                                normal = {textColor = Color.red},
                                active = {textColor = Color.red},
                                focused = {textColor = Color.red},
                                hover = {textColor = Color.red}
                            };
                        }
                        key.stringValue = EditorGUI.DelayedTextField(pos, key.stringValue, textStyle);
                        pos.height = rect.height - pos.height;
                        EditorGUI.PropertyField(pos, value, GUIContent.none, true);
                        serializedObject.ApplyModifiedProperties();
                    },
                    elementHeightCallback = listIndex =>
                    {
                        SerializedProperty value = _data.GetArrayElementAtIndex(listIndex)
                            .FindPropertyRelative(ResourceReferenceGenerator.VALUE_FIELD);
                        return EditorGUI.GetPropertyHeight(value, true) + 2.0f;
                    }
                },
                index => _data.GetArrayElementAtIndex(index)
                    .FindPropertyRelative(ResourceReferenceGenerator.KEY_FIELD).stringValue
            );
            
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            serializedObject.Update();
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            _duplicateCheck.Clear();
            _duplicatedHash.Clear();
            for (var i = 0; i < _data.arraySize; i++)
            {
                SerializedProperty element = _data.GetArrayElementAtIndex(i);
                int keyHash = element.FindPropertyRelative(ResourceReferenceGenerator.KEY_FIELD).stringValue.GetStableHash();
                if (_duplicateCheck.Add(keyHash) == false)
                {
                    _duplicatedHash.Add(keyHash);
                }
            }

            if (_duplicatedHash.Count > 0)
            {
                var errorLabel = new GUIContent(EditorIconProperties.ErrorIcon)
                {
                    text = "List contains duplicate hashes. Please remove or rename <color=red>red</color> entries."
                };
                var errorBoxStyle = new GUIStyle(EditorStyles.helpBox) {richText = true};
                EditorGUILayout.LabelField(errorLabel, errorBoxStyle);
            }
            _searchableList.DoLayoutList();
        }
    }
}